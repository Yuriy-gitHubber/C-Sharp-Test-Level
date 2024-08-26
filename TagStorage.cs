using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace C__Test
{
    internal class TagStorage
    {
        public TagItem Root { get; private set; } //Метод возврата и установки экземпляра класса TagItem - корневой тег

        public TagStorage() //Конструктор
        {
            Root = new TagItem("Root");
        }

        public TagItem FindTagByFullPath(string fullPath) //Метод поиска тега по его полному пути
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }

            string[] strings = fullPath.Split(new [] {'.'}, StringSplitOptions.RemoveEmptyEntries);

            TagItem current_tag = Root;

            foreach (var s in strings)
            {
               
                if(current_tag == null)
                {
                    return null;
                }
                current_tag = current_tag.GetChildTag(s);

                //if (current_tag != null)
                //{
                //    Console.WriteLine($"Текущий тег: {current_tag.TagName}, полный путь: {current_tag.FullPath}");
                //}
                //else
                //{
                //    Console.WriteLine($"Тег с именем '{s}' не найден в пути '{fullPath}'");
                //}

            }
            return current_tag;
     
        }
        public void SaveToFile(string fileName) //Метод сохранения дерева тегов в файл
        {
            var xml =  SaveTagToXml(Root);

            xml.Save(fileName,SaveOptions.None);
        }
        public void LoadFromFile(string fileName) //Метод выгрузки дерева тегов из файла
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Файл не найден", fileName);
            }

            var xml = XElement.Load(fileName);
            Root = LoadTagFromXml(xml, null);
        }
        private XElement SaveTagToXml(TagItem tag)//Метод сохранения тегов в xml файл
        {
            var element = new XElement(tag.TagName, tag.GetTagValue()?.ToString() ?? string.Empty);

            foreach (var child in tag.GetDirectChildren())
            {
                element.Add(SaveTagToXml(child));
            }

            return element;
        }
        private TagItem LoadTagFromXml(XElement element, TagItem parent)//Метод выгрузки дерева тегов из xml файла
        {
            string name = element.Name.LocalName;
            string value = element.Value;

            object tagValue = DetermineTagValue(value);

            var tag = new TagItem(name, tagValue, parent);

            foreach (var childElement in element.Elements())
            {
                var childTag = LoadTagFromXml(childElement, tag);
                tag.AddChildTag(childTag);
            }

            return tag;
        }
        private object DetermineTagValue(string value) // Метод для определения типа значения, переданного в виде строки, и преобразования его в соответствующий тип данных
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            else if (double.TryParse(value, out double doubleValue))
            {
                return doubleValue;
            }
            else if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<TagItem> GetAllTags() //Метод получения всех дочерних тегов
        {
            yield return Root;
            foreach (var child in Root.GetAllChildTags())
            {
                yield return child;
            }
        }


    }
}
