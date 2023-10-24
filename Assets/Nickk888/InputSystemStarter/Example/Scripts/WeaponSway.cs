using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float strength = 5;
    public float positionStrength = 0.005f;
    public float smooth = 5;

    private Quaternion localRotation;

    void Start()
    {
        localRotation = transform.localRotation;
    }
    
    void Update()
    {
        Vector2 lookInputValue = PlayerInput.Instance.GetLookInput();
        
        float rotationX = lookInputValue.x;
        float rotationY = -lookInputValue.y;

        float positionX = lookInputValue.x;
        float positionY = -lookInputValue.y;

        rotationX = Mathf.Clamp(rotationX, -strength, strength);
        rotationY = Mathf.Clamp(rotationY, -strength, strength);

        positionX = Mathf.Clamp(rotationX, -positionStrength, positionStrength);
        positionY = Mathf.Clamp(rotationY, -positionStrength, positionStrength);

        Quaternion newRotation = Quaternion.Euler(localRotation.x + rotationY, localRotation.y + rotationX, localRotation.z);
        Vector3 newPosition = new Vector3(positionX, positionY);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * smooth);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * smooth);
    }
}