using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMenuVerticalSize : MonoBehaviour
{
    // Value of all children.
    public float childHeight = 35f;


    // Start is called before the first frame update
    void Start()
    {
        //AdjustSize();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Readjusts size of container object to allow for given height.
    public void AdjustSize()
    {
        Vector2 size = this.GetComponent<RectTransform>().sizeDelta;
        size.y = this.transform.childCount * childHeight;
        this.GetComponent<RectTransform>().sizeDelta = size;
    }

}
