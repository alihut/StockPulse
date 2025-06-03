using Microsoft.EntityFrameworkCore;
using System.Text;

namespace StockPulse.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToDetailedString(this Exception ex)
        {
            if (ex == null) return string.Empty;

            var sb = new StringBuilder();

            void AppendExceptionDetails(Exception e, int level)
            {
                string indent = new string('>', level);
                sb.AppendLine($"{indent} Exception Type: {e.GetType().FullName}");
                sb.AppendLine($"{indent} Message       : {e.Message}");
                sb.AppendLine($"{indent} Source        : {e.Source}");
                sb.AppendLine($"{indent} TargetSite    : {e.TargetSite}");
                sb.AppendLine($"{indent} StackTrace    : {e.StackTrace}");

                if (e.Data != null && e.Data.Count > 0)
                {
                    sb.AppendLine($"{indent} Data:");
                    foreach (var key in e.Data.Keys)
                    {
                        sb.AppendLine($"{indent}   {key}: {e.Data[key]}");
                    }
                }

                if (e.InnerException != null)
                {
                    sb.AppendLine($"{indent} Inner Exception:");
                    AppendExceptionDetails(e.InnerException, level + 1);
                }
            }

            AppendExceptionDetails(ex, 0);
            return sb.ToString();
        }

        public static string ToDetailedString(this DbUpdateConcurrencyException ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("DbUpdateConcurrencyException caught:");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");

            foreach (var entry in ex.Entries)
            {
                sb.AppendLine($"- Entity: {entry.Entity.GetType().Name}");

                sb.AppendLine("  Original Values:");
                foreach (var prop in entry.OriginalValues.Properties)
                {
                    sb.AppendLine($"    {prop.Name}: {entry.OriginalValues[prop]}");
                }

                sb.AppendLine("  Current Values:");
                foreach (var prop in entry.CurrentValues.Properties)
                {
                    sb.AppendLine($"    {prop.Name}: {entry.CurrentValues[prop]}");
                }
            }

            return sb.ToString();
        }
    }

}
