using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcoreTransformersApp.Models
{
    public class TransformerDBContext : DbContext, ITransformerDBContext
    {
        public TransformerDBContext(DbContextOptions<TransformerDBContext> options)
            : base(options)
        { }

        public DbSet<Transformer> Transformers { get; set; }
        public DbSet<TransformerAllegiance> TransformerAllegiances { get; set; }
        public DbContext DatabaseContext { get { return ((DbContext)this); } }
    }

    public static class ModelBuilderExtensions
    {
        public static Task SeedData(this TransformerDBContext context)
        {
            return Task.Run(() =>
            {
                if (context.Database.IsSqlServer()) context.Database.EnsureCreated(); // For MSSQL only

                // To truncate tables and reseed the identities -- only required during odd cases
                //if (context.Transformers.Any())
                //{
                //    context.Transformers.RemoveRange(context.Transformers);
                //    context.TransformerAllegiances.RemoveRange(context.TransformerAllegiances);
                //    context.SaveChanges();
                //    if (context.Database.IsSqlServer()) // For MSSQL only
                //    {
                //        context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Transformers', RESEED, 0)");
                //        context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('TransformerAllegiances', RESEED, 0)");
                //    }
                //}

                if (!context.TransformerAllegiances.Any())
                {
                    context.TransformerAllegiances.AddRange(TransformersEnums.TransformerAllegiancesList());
                    context.SaveChanges();
                }

                if (!context.Transformers.Any())
                {
                    context.Transformers.AddRange(getTransformers(context).GetAwaiter().GetResult());
                    context.SaveChanges();
                }
            });
        }

        public static Task<List<Transformer>> getTransformers(TransformerDBContext context) {

            Func<int, int, int> getRandomNumber = (int min, int max) => new Random().Next(1, 10);

            return Task.Run(() => context
                                    .TransformerAllegiances
                                    .ToList<TransformerAllegiance>()
                                    .Select((transformerAllegiance, index) =>  
                                        Enumerable
                                            .Range(1,5)
                                            .Select(item => 
                                                new Transformer {
                                                        AllegianceId = transformerAllegiance.TransformerAllegianceId 
                                                        ,Name = $"{transformerAllegiance.AllegianceName} - {NumberToWords(item)}"
                                                        ,Strength = getRandomNumber(1,10)
                                                        ,Intelligence = getRandomNumber(1,10)
                                                        ,Speed = getRandomNumber(1,10)
                                                        ,Endurance = getRandomNumber(1,10)
                                                        ,Rank = getRandomNumber(1,10)
                                                        ,Courage = getRandomNumber(1,10)
                                                        ,Firepower = getRandomNumber(1,10)
                                                        ,Skill = getRandomNumber(1,10)
                                                    }
                                             )
                                             .ToList<Transformer>()
                                    )
                                    .SelectMany(items => items)
                                    .ToList<Transformer>()
           );
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }
}
