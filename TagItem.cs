using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Test
{
    internal class TagItem
    {
        private object _tag_value; //Значение тега
        private string _tag_name; //Имя тега

        private TagItem _parent_tag;//Родительский тег
        public TagItem ParentTag //Свойство установки и возврата значения 
        {
            get { return _parent_tag; }
            private set { _parent_tag = value; } 
        }

        private Dictionary<string, TagItem> _children_tags; //Словарь дя хранения тегов

        public Type valueType => _tag_value?.GetType() ?? typeof(void); //Метод возврата типа значения хранимого тегом

        public Object GetTagValue() { return _tag_value; }// Метод возврата значения тега
        public string TagName //Свойство возврата и установки Имени тега
        {
            get
            {
                return _tag_name;
            }
            set
            {
                if (_parent_tag != null)
                {
               
                    _parent_tag._children_tags.Remove(_tag_name);

                    
                    _tag_name = value;

                
                    _parent_tag._children_tags[_tag_name] = this;
                }
                else
                {
                    _tag_name = value;
                }


            } 
        }

        public void SetTagValue(object value) { _tag_value = value; } //Метод установки значения тега

        public TagItem(string tag_name, object tag_value = null, TagItem parent_tag = null) //Конструктор 
        {
            TagName = tag_name;
            _tag_value = tag_value;
            _parent_tag = parent_tag;
            _children_tags = new Dictionary<string, TagItem>();
            Level = _parent_tag != null ? _parent_tag.Level + 1 : 1;
            UpdateFullpath();
        }

        public int Level { get; private set; } //Свойство установки и возврата уровня вложенности

        public string FullPath { get; set; } //Свойство установки и возврата полного пути тега

       public void UpdateFullpath() //Метод обновления полного пути тега
        {

            // Обновляем полный путь для текущего тега
            FullPath = _parent_tag != null ? $"{_parent_tag.FullPath}.{TagName}" : TagName;

            //Console.WriteLine($"Путь обновлен: {FullPath}");

            // Рекурсивно обновляем полный путь для всех дочерних тегов
            foreach (var child in _children_tags.Values)
            {
                child.UpdateFullpath();
            }
        }


        public TagItem GetChildTag(string tag_name) //Метод возврата дочерних тегов
        {
            return _children_tags.ContainsKey(tag_name) ? _children_tags[tag_name] : null;
        }

        public void AddChildTag(TagItem child_tag) //Метод добавления дочерних тегов
        {
            if(child_tag != null && !_children_tags.ContainsKey(child_tag.TagName)) 
            {
                _children_tags[child_tag.TagName] = child_tag;
                child_tag._parent_tag = this;
                child_tag.Level = this.Level + 1;
                child_tag.UpdateFullpath() ;
            }
        }
        
        public void RemoveChildTag(string child_tag_name) //Метод удаления тега
        {
            if (_children_tags.ContainsKey(child_tag_name)){

                _children_tags.Remove(child_tag_name);
            }
        }
        public IEnumerable<TagItem> GetDirectChildren() //Метод возврата прямых дочерних тегов
        {
            return _children_tags.Values;
        }
        public IEnumerable<TagItem> GetAllChildTags() //Рекурсивный метод возврата всех потомков текущего тега
        {
            foreach(var child in _children_tags.Values)
            { 
                yield return child;
                foreach (var descendant in child.GetAllChildTags())
                {
                    yield return descendant;
                }

            }
        }

    }
}
