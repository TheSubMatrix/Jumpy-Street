#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SerializableQueue<>))]
public class SerializableQueueDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new();
        SerializedProperty queueProperty = property.FindPropertyRelative("m_queue");
        
        ListView listView = new()
        {
            showBoundCollectionSize = true,
            showFoldoutHeader = true,
            showAddRemoveFooter = true,
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            headerTitle = property.displayName,
            showBorder = true
        };
        
        listView.BindProperty(queueProperty);
        
        listView.makeItem = () => new PropertyField();
        
        listView.bindItem = (element, index) =>
        {
            PropertyField propertyField = element as PropertyField;
            SerializedProperty arrayElement = queueProperty.GetArrayElementAtIndex(index);
            propertyField.BindProperty(arrayElement);
            if (propertyField != null) propertyField.label = GetOrdinalLabel(index);
        };
        listView.itemsAdded += _ => listView.RefreshItems();
        listView.itemsRemoved += _ => listView.RefreshItems();
        listView.itemIndexChanged += (_, _) => listView.RefreshItems();
        
        container.Add(listView);
        return container;
    }
    
    static string GetOrdinalLabel(int index)
    {
        int num = index + 1;
        if (num % 100 >= 11 && num % 100 <= 13)
            return num + "th";
        
        return (num % 10) switch
        {
            1 => num + "st",
            2 => num + "nd",
            3 => num + "rd",
            _ => num + "th"
        };
    }
}
#endif