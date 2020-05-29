using DataMigration.Models;
using ShellProgressBar;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataMigration
{
    public class Migration : IMigration
    {
        private int totalCount = 0;
        private static readonly object LockObj = new object();
        private int numOfProcesses = 0;

        private static JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

        private static ProgressBarOptions ProgressBarOptions =>
            new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };

        private static ProgressBarOptions ChildProgressBarOptions =>
            new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.DarkBlue,
                BackgroundColor = ConsoleColor.Yellow,
                ProgressCharacter = '─',
                CollapseWhenFinished = false
            };

        private readonly SourceDbContext ctx1;
        private readonly HostDbContext ctx2;
        private readonly ConfigModel model;
        private int TotalTicks { get; set; }

        public Migration()
        {
            var jsonString = File.ReadAllText("database.json");
            model = JsonSerializer.Deserialize<ConfigModel>(jsonString, JsonOptions);
            ctx1 = new SourceDbContext(model.SourceConfig.ConnectionString);
            ctx2 = new HostDbContext(model.HostConfig.ConnectionString);
        }

        public void TestEF()
        {
            var skill = ctx1.Skills.FirstOrDefault(x => x.Id == 2);
            Console.WriteLine("EF test");
            Console.WriteLine(skill?.Skill);
        }

        public void MigrateUsers()
        {
            var members = ctx1.Members.ToList();
            TotalTicks = members.Count;

            Console.WriteLine($"Members to migrate: {TotalTicks}");

            using var pbar = new ProgressBar(TotalTicks, "Data migration progress", ProgressBarOptions);
            ctx2.Database.BeginTransaction();
            var ct = 0;
            foreach (var member in members)
            {
                pbar.Message = $"Start {ct} of {TotalTicks}";
                var user = ctx2.User.FirstOrDefault(x => x.Phone == member.Phone.ToString());
                if (user == null)
                {
                    var skill = member.SkillId != null ? ctx1.Skills.FirstOrDefault(x => x.Id == member.SkillId)?.Skill : "brak wyszkolenia";
                    var newUser = new User
                    {
                        Phone = member.Phone.ToString(),
                        Password = member.Token,
                        Roles = "[]",
                        Skill = skill
                    };
                    ctx2.User.Add(newUser);
                    ctx2.SaveChanges();
                }
                pbar.Tick($"Inserted rows {ct} of {TotalTicks}");
                ct++;
            }
            ctx2.Database.CommitTransaction();
        }

        public void MigrateUnits()
        {
            IList<OldUnits> units = ctx1.Units.ToList();
            if (units.Count <= 0) return;
            double frac = units.Count % 20;
            numOfProcesses = (int)Math.Ceiling(frac);
            var limit = units.Count / numOfProcesses;

            MakeProgressBar<OldUnits>(ref units, limit, childAction: (x) => MigrateUnit((OldUnits)x));
        }

        public void MigrateAssociation()
        {
            IList<MemberAssociation> members = ctx1.MemberAssociation.ToList();
            if (members.Count <= 0) return;
            double frac = members.Count % 10;
            numOfProcesses = (int)Math.Ceiling(frac);
            var limit = members.Count / numOfProcesses;

            MakeProgressBar<MemberAssociation>(ref members, limit, childAction: (x) => MigrateUserAssociation((MemberAssociation)x));
        }

        private void MakeProgressBar<T>(ref IList<T> items, int limit, Action<object> childAction = null)
        {
            IList<Task> tasks = new List<Task>();

            using var pbar = new ProgressBar(numOfProcesses, "Migration", ProgressBarOptions);
            for (var i = 0; i < numOfProcesses; i++)
            {
                limit = limit > items.Count ? items.Count : limit;
                var part = items.Take(limit).ToList();
                items = items.Skip(limit).ToList();

                tasks.Add(Task.Run(() => ChildProcess(pbar, part, childAction)));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private void ChildProcess(IProgressBar pbar, ICollection items, Action<object> childAction = null)
        {
            var childMax = items.Count;
            var counter = 1;
            using var child = pbar.Spawn(childMax, "Child process", ChildProgressBarOptions);
            foreach (var item in items)
            {
                child.Tick($"done {counter} of {childMax}");
                childAction?.Invoke(item);
                counter++;
            }
            lock (LockObj)
            {

            }
            totalCount++;
            pbar.Tick($"Ended {totalCount} of {numOfProcesses}");
        }

        private static void MigrateUserAssociation(MemberAssociation item)
        {
            var db1 = new SourceDbContext();
            var db2 = new HostDbContext();
            var member = db1.Members.FirstOrDefault(m => m.MemberId == item.MemberId);
            if (member != null)
            {
                var user = db2.User.FirstOrDefault(x => x.Phone == member.Phone.ToString());
                var unit = db2.Units.FirstOrDefault(x => x.UnitId == item.UnitId);
                if (user != null && unit != null)
                {
                    var isAssociated = db2.UserAssociation.FirstOrDefault(x => x.UnitId == unit.Id && x.IdUser == user.Id);
                    if (isAssociated == null)
                    {
                        var ua = new UserAssociation
                        {
                            IdUser = user.Id,
                            UnitId = unit.Id
                        };
                        db2.UserAssociation.Add(ua);
                        db2.SaveChanges();
                    }
                }
            }
            db1.Dispose();
            db2.Dispose();
        }

        private void MigrateUnit(OldUnits unit)
        {
            var u = ctx2.Units.FirstOrDefault(x => x.UnitId == unit.UnitId);
            if (u != null) return;
            var o = new Units
            {
                UnitId = unit.UnitId,
                Description = unit.Description,
                Phone = unit.Phone
            };
            ctx2.Units.Add(o);
            ctx2.SaveChanges();
        }

        public ConfigModel GetConfig()
        {
            return model;
        }
    }
}
