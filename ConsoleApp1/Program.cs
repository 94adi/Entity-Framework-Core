using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            RawSqlCommandUpdateSamurai();
            QuerrySamurais();
            Console.ReadKey();
        }

        private static void AddAndUpdateBattle()
        {
            _context.Battles.Add(new Battle() { Name = "Okizawa", StartDate = new DateTime(1456, 11, 23), EndDAte = new DateTime(1457, 2, 11) });
            _context.SaveChanges();
            var battle = _context.Battles.FirstOrDefault();
            _context.Battles.Update(battle);
            _context.SaveChanges();

        }

        private static void QuerryBattles()
        {
            var battles = _context.Battles.ToList();
            foreach(var battle in battles)
            {
                Console.WriteLine("Name of battle: " + battle.Name);
                Console.WriteLine("Start date of battle: " + battle.StartDate.ToString());
                Console.WriteLine("End date of battle: " + battle.EndDAte.ToString());
                Console.WriteLine("===============================");
            }
        }

        private static void InsertMultipleSamurais()
        {

            var samurai1 = new Samurai { Name = "Steve" };
            var samurai2 = new Samurai { Name = "Jack" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.AddRange(new List<Samurai>() {samurai1,samurai2 });
                context.SaveChanges();
            }
        }

        private static void InsertSamurai(string name)
        {
            var samurai = new Samurai { Name = name };
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }

        private static void QuerrySamurais()
        {
            using (var context = new SamuraiContext())
            {
                var samurais = context.Samurais.ToList();
                foreach (var samurai in samurais)
                {
                    Console.WriteLine(samurai.Id + " " + samurai.Name);
                }
            }
        }

        private static void SamuraiQuery(string name)
        {
            var samurais = _context.Samurais.Where(n => n.Name == name).ToList();
            foreach(var samurai in samurais)
            {
            Console.WriteLine(samurai.Id + " " + samurai.Name);
            }
        }

        private static bool AddSamurai(Samurai s)
        {
            if(s != null)
            {
                _context.Samurais.Add(s);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        private static void QueryAndUpdateSamurai()
        {
            //NOTE: When using the same context to retrieve and update an entity, Update() will recieve only the edited fields
            //      When using a different context to call Update() and pass the samurai object modified with a previous, different context, it will send ALL
            //      non null fields to be updated, not just the ones you previously modified
            var samurai = _context.Samurais.Last(s => s.Name == "Adi");
            samurai.Name += " Cotuna";
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Update(samurai);
                newContext.SaveChanges();
            }
        }

        private static void RawSqlQuery()
        {
            var samurais = _context.Samurais.FromSql("Select * from Samurais")
                .OrderByDescending(s => s.Name)
                .Where(s=>s.Name.Contains("Adi"))
                .ToList();
            samurais.ForEach(s => Console.WriteLine(s.Name));
        }

        private static void RawSqlCommandUpdateSamurai()
        {
            var updatedSamurais = _context.Database.ExecuteSqlCommand(
                "UPDATE samurais SET Name = REPLACE(Name,'Adi','Adrian')");
            Console.WriteLine($"Affected samurais {updatedSamurais}");
        }
    }
}
