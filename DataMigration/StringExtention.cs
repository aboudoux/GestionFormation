namespace DataMigration
{
    public static class StringExtention
    {
        public static bool IsEmpty(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }
    }
}