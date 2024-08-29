internal class TagItem
{
    private object _tagValue; // Значение тега
    private string _tagName; // Имя тега
    private TagItem _parentTag; // Родительский тег
    private Dictionary<string, TagItem> _childrenTags; // Словарь для хранения дочерних тегов
    public TagItem ParentTag //Свойство установки и возврата значения 
    {
        get { return _parentTag; }
        private set { _parentTag = value; }
    }
    public Type ValueType => _tagValue?.GetType() ?? typeof(void); // Метод возврата типа значения хранимого тегом

    public object GetTagValue() { return _tagValue; } // Метод возврата значения тега

    public string TagName
    {
        get { return _tagName; }
        set
        {
            if (_parentTag != null)
            {
                _parentTag._childrenTags.Remove(_tagName);
                _tagName = value;
                _parentTag._childrenTags[_tagName] = this;
            }
            else
            {
                _tagName = value;
            }
        }
    }

    public void SetTagValue(object value) { _tagValue = value; } // Метод установки значения тега

    public TagItem(string tagName, object tagValue = null, TagItem parentTag = null) // Конструктор 
    {
        TagName = tagName;
        _tagValue = tagValue;
        _parentTag = parentTag;
        _childrenTags = new Dictionary<string, TagItem>();
        Level = _parentTag != null ? _parentTag.Level + 1 : 1;
        UpdateFullPath();
    }

    public int Level { get; private set; } // Свойство уровня вложенности

    public string FullPath { get; set; } // Свойство полного пути тега

    public void UpdateFullPath() // Метод обновления полного пути тега
    {
        FullPath = _parentTag != null ? $"{_parentTag.FullPath}.{TagName}" : TagName;

        foreach (var child in _childrenTags.Values)
        {
            child.UpdateFullPath();
        }
    }

    public TagItem GetChildTag(string tagName) // Метод возврата дочерних тегов
    {
        return _childrenTags.ContainsKey(tagName) ? _childrenTags[tagName] : null;
    }

    public void AddChildTag(TagItem childTag) // Метод добавления дочерних тегов
    {
        if (childTag != null && !_childrenTags.ContainsKey(childTag.TagName))
        {
            _childrenTags[childTag.TagName] = childTag;
            childTag._parentTag = this;
            childTag.Level = this.Level + 1;
            childTag.UpdateFullPath();
        }
    }

    public void RemoveChildTag(string childTagName) // Метод удаления тега
    {
        if (_childrenTags.ContainsKey(childTagName))
        {
            _childrenTags.Remove(childTagName);
        }
    }

    public IEnumerable<TagItem> GetDirectChildren() // Метод возврата прямых дочерних тегов
    {
        return _childrenTags.Values;
    }

    public IEnumerable<TagItem> GetAllChildTags() // Рекурсивный метод возврата всех потомков текущего тега
    {
        foreach (var child in _childrenTags.Values)
        {
            yield return child;
            foreach (var descendant in child.GetAllChildTags())
            {
                yield return descendant;
            }
        }
    }
}
