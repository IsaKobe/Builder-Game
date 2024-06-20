using System.Collections.Generic;

public class Storage : Building
{
    public List<bool> canStore = new();
    protected override void Awake()
    {
        base.Awake();
        for(int i = 0; i < build.localRes.ammount.Length; i++)
        {
            canStore.Add(true);
        }
    }
}
