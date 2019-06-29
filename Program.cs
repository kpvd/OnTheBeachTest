using System;
using System.Collections.Generic;
using System.Linq;

namespace OnTheBeachCodeTest
{
    class Job
    {
        public readonly string Name;
        public readonly string DependsOn;

        public Job(string name, string dependsOn)
        {
            Name = name;
            DependsOn = dependsOn;
        }
    }

    class Program
    {
        static string TestJobs(string input)
        {
            //Parse the string and create the 'Job' object
            var jobsStrings = input.Split(',',StringSplitOptions.RemoveEmptyEntries);

            var jobs = new List<Job>();
            foreach (var jobString in jobsStrings)
            {
                var job = jobString.Trim().Split("=>", StringSplitOptions.RemoveEmptyEntries);
                if(job.Length==1)
                    jobs.Add(new Job(job[0].Trim(), null));
                else
                    jobs.Add(new Job(job[0].Trim(), job[1].Trim()));
            }
            

            var result = jobs.Select(job => job.Name).ToList();
            var dependancy = new Dictionary<string, List<string>>();
            foreach (var job in jobs)
            {
                if (job.Name == job.DependsOn)
                    throw new Exception("Jobs can’t depend on themselves");
                if (job.DependsOn != null)
                {
                    if(dependancy.ContainsKey(job.DependsOn))
                        dependancy[job.DependsOn].Add(job.Name);
                    else
                        dependancy.Add(job.DependsOn, new List<string>() {job.Name });

                    var dependsOnIndex = result.IndexOf(job.DependsOn);
                    result.RemoveAt(dependsOnIndex);

                    var dependancyList = dependancy[job.DependsOn];

                    //Get the lowest index in case multiple jobs depends on the same job
                    var insertIndex= dependancyList.Min(x => result.IndexOf(x));

                    result.Insert(insertIndex, job.DependsOn);
                }
            }

            //Check for circular reference
            foreach (var job in jobs)
            {
                if (job.DependsOn != null)
                {
                    var nameIndex = result.IndexOf(job.Name);
                    var dependsOnIndex = result.IndexOf(job.DependsOn);
                    if (dependsOnIndex > nameIndex)
                    {
                        throw new Exception("Circular reference : @ " + job.Name + "=>" + job.DependsOn);
                    }
                }
            }

            return string.Join(" ",result);
        }
        static void Main(string[] args)
        {
            var result = TestJobs("a =>");
            Console.WriteLine(result);

            result = TestJobs("a =>,b =>,c =>");
            Console.WriteLine(result);

            result = TestJobs("a =>,b => c,c =>");
            Console.WriteLine(result);

            result = TestJobs("a =>, b => c, c => f, d => a, e => b, f =>");
            Console.WriteLine(result);

            result = TestJobs("a =>, b => f, c => e, d => c, e => a, f =>");
            Console.WriteLine(result);

            result = TestJobs("a =>f, b =>, c =>e, d => a, e =>, f =>b,g=>c");
            Console.WriteLine(result);

            //Multiple job depends on the same job
            result = TestJobs("a =>e, b =>e, c =>e, d => , e =>");
            Console.WriteLine(result);

            try
            {
                result = TestJobs("a =>,b =>,c =>c");
            }
            catch (Exception e)
            {
                //NOTE:Swallow the exception
                Console.WriteLine(e.Message);
            }

            try
            {
                 result = TestJobs("a =>,b => c,c => b,d => e,e =>b");
            }
            catch (Exception e)
            {
                //NOTE:Swallow the exception
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
            }
        }
}
