using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace C_971.Models
{
    public partial class Course : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _name = string.Empty;
        [NotNull]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private DateTime _endDate = DateTime.Today.AddMonths(6);
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private CourseStatus _status = CourseStatus.Planned;
        public CourseStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private bool _startDateNotifications = false;
        public bool StartDateNotifications
        {
            get => _startDateNotifications;
            set => SetProperty(ref _startDateNotifications, value);
        }

        private bool _endDateNotifications = false;
        public bool EndDateNotifications
        {
            get => _endDateNotifications;
            set => SetProperty(ref _endDateNotifications, value);
        }

        private int _creditUnits = 3;
        public int CreditUnits
        {
            get => _creditUnits;
            set => SetProperty(ref _creditUnits, value);
        }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign keys
        private int _termId;
        [NotNull]
        public int TermId
        {
            get => _termId;
            set => SetProperty(ref _termId, value);
        }

        private int? _instructorId;
        public int? InstructorId
        {
            get => _instructorId;
            set => SetProperty(ref _instructorId, value);
        }

        // Navigation properties - ignored by SQLite
        [Ignore]
        public AcademicTerm Term { get; set; }

        [Ignore]
        public CourseInstructor Instructor { get; set; }

        [Ignore]
        public List<CourseAssessment> Assessments { get; set; } = new();

        [Ignore]
        public List<CourseNote> Notes { get; set; } = new();

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}