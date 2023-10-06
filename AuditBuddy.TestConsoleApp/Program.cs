using System;
using AuditBuddy.Library;

namespace AuditBuddyTestConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new AdvancedAuditPolicyManager();
            var categories = manager.GetAdvancedAuditSubCategories();
            Console.WriteLine($"Found {categories.Count} Categories");

            var policies = manager.GetAdvancedAuditPolicies();
            Console.WriteLine($"Found {policies.Count} Policies");

            foreach (AdvancedAuditPolicy policy in policies)
            {
                if (policy.Setting == AuditingFlags.None)
                {
                    Console.WriteLine($"The following audit setting are not enabled: Category [{policy.Category}] SubCategory [{policy.SubCategory}]");
                }
            }
        }
    }
}
