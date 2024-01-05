using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenCounter
{
    #region SELECTED

    public void CounterSelected();

    public void CounterDeselected();


    #endregion

    #region INTERACT

    public void Interact(Player player);

    public void AlternativeInteract(Player player);

    #endregion

}
