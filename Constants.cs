namespace C_971
{
    public static class Constants
    {
        public const string DatabaseFilename = "CollegeCourseTracker.db3";
        public const string AiAssistantTitle = "Ask Codex";
        public const string OpenAiDefaultModel = "gpt-4.1-mini";
        public const string OpenAiApiKeyStorageKey = "OpenAiApiKey";
        public const string OpenAiApiKeyPortalUrl = "https://platform.openai.com/settings/organization/api-keys";
        public const string UniversityOfThePeopleUrl = "https://www.uopeople.edu/";
        public const string UniversityOfThePeopleReminderTitle = "Explore University of the People";
        public const string UniversityOfThePeopleReminderText =
            "An accredited online university that describes itself as tuition-free; fees may apply.";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
