using UnityEngine.UIElements;

public abstract class Slot<ContentType>
{
    public delegate void SlotClicked(Slot<ContentType> slot);
    static public event SlotClicked SlotClickedEvent;
    static public event SlotClicked SlotRightClickedEvent;

    private ContentType myContent;

    public abstract bool ValidateContent(ContentType possibleContent);
    
    public ContentType GetContent()
    {
        return myContent;
    }

    public void SlotContent(ContentType newContent)
    {
        myContent = newContent;
    }

    public void Unslot()
    {

    }
}