using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using SimpleMvvmToolkit;
using System.ComponentModel;

namespace AKBMatome.Data
{
    [DataContract]
    [Table]
    public class FeedGroup : ModelBase<FeedGroup>
    {
        [Column(IsVersion = true)]
#pragma warning disable
        private Binary version;
#pragma warning restore

        private int _Id;
        [DataMember(Name = "id")]
        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false)]
        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
                NotifyPropertyChanged(m => Id);
            }
        }

        private string _Title;
        [DataMember(Name = "title")]
        [Column]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                NotifyPropertyChanged(m => Title);
            }
        }
    }
}
