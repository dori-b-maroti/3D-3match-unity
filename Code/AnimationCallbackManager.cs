/*Property of Dorothea "Dori" B-Maroti
----All rights reserved----*/

using System.Collections.Generic;
using System;
public class AnimationCallbackManager
{
    public event Action<int> dropCompleted;
    public int destroyedTileIndex;

    private List<int> animatedTileIndices = new List<int>();

    public void AddCallbackId(int id)
    {
        animatedTileIndices.Add(id);
    }

    public void CompleteDropOnTile(int id)
    {
        if (animatedTileIndices.Count == 0)
        {
            dropCompleted(destroyedTileIndex);
        }
        else
        {
            animatedTileIndices.Remove(id);
        }
    }
}
