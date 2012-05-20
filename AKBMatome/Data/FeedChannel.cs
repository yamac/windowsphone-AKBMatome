using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using SimpleMvvmToolkit;

namespace AKBMatome.Data
{
    [DataContract]
    [Table]
    public class FeedChannel : ModelBase<FeedChannel>
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

        private int _FeedGroupId;
        [DataMember(Name = "feed_group_id")]
        [Column]
        public int FeedGroupId
        {
            get { return _FeedGroupId; }
            set
            {
                if (_FeedGroupId == value) return;
                _FeedGroupId = value;
                NotifyPropertyChanged(m => FeedGroupId);
            }
        }

        private string _FeedLink;
        [DataMember(Name = "feed_link")]
        [Column]
        public string FeedLink
        {
            get { return _FeedLink; }
            set
            {
                if (_FeedLink == value) return;
                _FeedLink = value;
                NotifyPropertyChanged(m => FeedLink);
            }
        }

        private string _AuthorName;
        [DataMember(Name = "author_name")]
        [Column]
        public string AuthorName
        {
            get { return _AuthorName; }
            set
            {
                if (_AuthorName == value) return;
                _AuthorName = value;
                NotifyPropertyChanged(m => AuthorName);
            }
        }

        private string _Link;
        [DataMember(Name = "link")]
        [Column]
        public string Link
        {
            get { return _Link; }
            set
            {
                if (_Link == value) return;
                _Link = value;
                NotifyPropertyChanged(m => Link);
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

        private int _AccentColor;
        [Column]
        public int AccentColor
        {
            get { return _AccentColor; }
            set
            {
                if (_AccentColor == value) return;
                _AccentColor = value;
                this.NotifyPropertyChanged(m => AccentColor, propertyChanged);
            }
        }

        private int _Priority;
        [Column]
        public int Priority
        {
            get { return _Priority; }
            set
            {
                if (_Priority == value) return;
                _Priority = value;
                this.NotifyPropertyChanged(m => Priority, propertyChanged);
            }
        }
    }
}
