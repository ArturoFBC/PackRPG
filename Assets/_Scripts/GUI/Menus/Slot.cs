using UnityEngine.UIElements;

public abstract class Slot<ContentType>
{
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