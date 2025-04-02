public enum TypeState 
{ 
    Play, 
    Eat, 
    Toilet, 
    Sleep 
}

public class Human : State
{
    public DataAgent _DataAgent;

     
    public override void LocadComponent()
    {
        base.LocadComponent();
        _DataAgent = GetComponent<DataAgent>();
    }
}
