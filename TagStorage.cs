using System.Globalization;
using System.Xml.Linq;

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

        string[] segments = fullPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        TagItem currentTag = Root;

        foreach (var s in segments)
        {
            if (currentTag == null)
            {
                return null;
            }
            currentTag = currentTag.GetChildTag(s);
        }
        return currentTag;
    }

    public void SaveToFile(string fileName) //Метод сохранения дерева тегов в файл
    {
        var xml = SaveTagToXml(Root);
        xml.Save(fileName, SaveOptions.None);
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

    private XElement SaveTagToXml(TagItem tag) //Метод сохранения тегов в xml файл
    {
        var element = new XElement(tag.TagName);
        if (!tag.GetDirectChildren().Any())
        {
            if (tag.GetTagValue() != null)
            {
                if (tag.GetTagValue() is double doubleValue)
                {
                    element.Value = doubleValue.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    element.Value = tag.GetTagValue().ToString();
                }
            }
        }
        foreach (var child in tag.GetDirectChildren())
        {
            element.Add(SaveTagToXml(child));
        }

        return element;
    }

    private TagItem LoadTagFromXml(XElement element, TagItem parent) //Метод выгрузки дерева тегов из xml файла
    {
        string name = element.Name.LocalName;
        string value = element.HasElements ? null : element.Value;

        // Парсим значение тега
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
        else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
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
