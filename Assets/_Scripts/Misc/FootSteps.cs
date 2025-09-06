using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] GameObject footStep;
    float stepDuration = 4f;

    Vector3 right = new Vector3(0.1f, 0, 0);
    Vector3 left = new Vector3(-0.1f, 0, 0);
    Vector3 up = new Vector3(0, 0.1f, 0);
    Vector3 down = new Vector3(0, -0.1f, 0);

    Quaternion Direction = Quaternion.Euler(0, 0, 90);

    public void LeftFoot_Vertical()
    {
        Debug.Log(gameObject.name + "Left Vertical");
        GameObject step = Instantiate(footStep, transform.position + right, transform.rotation);
        Destroy(step, stepDuration);
    }

    public void RightFoot_Vertical()
    {
        Debug.Log(gameObject.name + "Right Vertical");
        GameObject step = Instantiate(footStep, transform.position + left, transform.rotation);
        Destroy(step, stepDuration);
    }

    public void LeftFoot_Horizontal()
    {
        Debug.Log(gameObject.name + "Left Horizontal");
        GameObject step = Instantiate(footStep, transform.position + up, Direction);
        Destroy(step, stepDuration);
    }

    public void RightFoot_Horizontal()
    {
        Debug.Log(gameObject.name + "Right Horizontal");
        GameObject step = Instantiate(footStep, transform.position + down, Direction);
        Destroy(step, stepDuration);
    }
}
