using UnityEngine;

public class BreathingButtons : MonoBehaviour
{
    [Header("Target Objects")]
    [Tooltip("Change the 'Size' number in the Inspector, then drag your buttons into the slots")]
    public Transform[] buttonsToAnimate; 

    [Header("Breathing Settings")]
    public float speed = 3f;        // How fast they breathe
    public float scaleAmount = 0.05f; // How much they grow/shrink

    // This array will remember the unique starting size of every button
    private Vector3[] startScales;

    void Start()
    {
        // Set the size of our memory array to match how many buttons we have
        startScales = new Vector3[buttonsToAnimate.Length];

        // Loop through the list and save the original size of each button
        for (int i = 0; i < buttonsToAnimate.Length; i++)
        {
            if (buttonsToAnimate[i] != null)
            {
                startScales[i] = buttonsToAnimate[i].localScale;
            }
            else
            {
                Debug.LogWarning($"BreathingButtons: The button slot at index {i} is empty!");
            }
        }
    }

    void Update()
    {
        // We only need to do the math once per frame
        float wave = Mathf.Sin(Time.time * speed); 
        float pulse = wave * scaleAmount;

        // Loop through the list and apply the breathing effect to all of them
        for (int i = 0; i < buttonsToAnimate.Length; i++)
        {
            if (buttonsToAnimate[i] != null)
            {
                buttonsToAnimate[i].localScale = startScales[i] + new Vector3(pulse, pulse, pulse);
            }
        }
    }
}