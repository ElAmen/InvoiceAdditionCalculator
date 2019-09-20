using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace InvoiceAdditionCalculator
{
    class Program
    {


        static void Main(string[] args)
        {
            var sub = 100;

            Console.WriteLine($"\n\n\n\n----------EXCLUSIVE FOR {sub}-------------");
            var test1 = new IAProcessor(false).ProcessInvAdditions(Convert.ToDecimal(sub));


            Console.WriteLine($"\n\n----------INCLUSIVE {test1}-------------");
            var test2 = new IAProcessor(true).ProcessInvAdditions(test1);


            Console.ReadKey();
        }

        public decimal TopDown()
        {
            return 0;
        }
    }

    public class IAProcessor
    {
        private class InvoiceAddition
        {
            public bool IsInclusive { get; set; }
            public int Sequence { get; set; }
            public string Name { get; set; }
            public bool IsFlatFee { get; set; }
            public decimal Figure { get; set; }
            public decimal Amount { get; set; }
            public CalculationMethod CalculationMethod { get; set; }
        }

        public IAProcessor(bool incl = false)
        {
            InvoiceAdditions = new List<InvoiceAddition>
            {
                new InvoiceAddition{  Sequence=1, Name= "NHIL",  IsFlatFee=false, Figure=2.5M, IsInclusive=incl, CalculationMethod=CalculationMethod.CalculateFromItemSubtolal },
                new InvoiceAddition{  Sequence=2, Name= "GetFund",  IsFlatFee=false, Figure=2.5M, IsInclusive=incl, CalculationMethod=CalculationMethod.CalculateFromItemSubtolal },
                new InvoiceAddition{  Sequence=3, Name= "VAT",  IsFlatFee=false, Figure=12.5M, IsInclusive=incl, CalculationMethod=CalculationMethod.CalculateFromPrecedingSubtolal },
                new InvoiceAddition{  Sequence=4, Name= "T. Levy",  IsFlatFee=false, Figure=1M, IsInclusive=incl, CalculationMethod=CalculationMethod.CalculateFromItemSubtolal },
                new InvoiceAddition{  Sequence=5, Name= "Due",  IsFlatFee=true, Figure=10M, IsInclusive=incl, CalculationMethod=CalculationMethod.CalculateFromItemSubtolal },
 };
        }

        private ICollection<InvoiceAddition> InvoiceAdditions { get; set; }

        private decimal CalculateAmount(InvoiceAddition ia, decimal target = 0)
        {
            if (ia.IsFlatFee)
            {
                return ia.Figure;
            }
            return ia.Figure * target / 100;
        }

        public decimal ProcessInvAdditions(decimal Base)
        {
            var IAs = InvoiceAdditions.OrderBy(r => r.Sequence).ToList();

            decimal PreceedingSub = Base;

            var nBase = Base;

            if (IAs.FirstOrDefault().IsInclusive)
            {
                var rate = 1M;
                foreach (var ia in IAs)
                {
                    if (ia.CalculationMethod == CalculationMethod.CalculateFromItemSubtolal)
                    {
                        //TODO
                        //Flat rate before %ge
                        if (ia.IsFlatFee)
                        {
                            //rate += 1 / ia.Figure;
                        }
                        else
                        {
                        }
                        rate += ia.Figure / 100;
                    }
                    else
                    {
                        //TODO
                        //Flat rate before %ge
                        if (ia.IsFlatFee)
                        {
                            //rate = rate * 1 / ia.Figure;
                        }
                        else
                        {
                        }
                        rate = rate * (1 + (ia.Figure / 100));
                    }
                }

                nBase = Base / rate;
                PreceedingSub = nBase;
            }

            Console.WriteLine($"SubTotal =\t\t{nBase}");

            Console.WriteLine($"-----------------------------------");

            foreach (var ia in IAs)
            {
                string percent = ia.IsFlatFee ? "" : "%";

                if (ia.CalculationMethod == CalculationMethod.CalculateFromItemSubtolal)
                {
                    ia.Amount = CalculateAmount(ia, nBase);
                }
                else
                {
                    Console.WriteLine($"-----------------------------------");
                    Console.WriteLine($"SubTotal =\t\t{PreceedingSub}");

                    ia.Amount = CalculateAmount(ia, PreceedingSub);
                }

                PreceedingSub += ia.Amount;

                Console.WriteLine($"{ia.Name}\t {ia.Figure}{percent}\t\t{ia.Amount}");
            }
            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"Total =\t\t\t{PreceedingSub}");

            return PreceedingSub;
        }
    }

    public enum CalculationMethod
    {
        [Display(Name = "Calculate from item subtotal")]
        CalculateFromItemSubtolal,
        [Display(Name = "Calculate from preceding subtotal")]
        CalculateFromPrecedingSubtolal,
    }
}
