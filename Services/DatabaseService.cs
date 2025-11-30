using C_971.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task InitializeAsync()
        {
            if (_database is not null) return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "EduTrack.db");
            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<AcademicTerm>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<CourseNote>();
            await _database.CreateTableAsync<CourseAssessment>();
        }

        // Terms
        public async Task<List<AcademicTerm>> GetTermsAsync()
        {
            await InitializeAsync();
            return await _database.Table<AcademicTerm>().ToListAsync();
        }

        public async Task<int> SaveTermAsync(AcademicTerm term)
        {
            await InitializeAsync();
            return term.Id != 0
                ? await _database.UpdateAsync(term)
                : await _database.InsertAsync(term);
        }

        // Similar methods for Course, CourseNote, CourseAssessment...
    }
}
