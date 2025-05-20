using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] GameObject runSmoke;
    [SerializeField] GameObject footStep;
    float stepDuration = 4f;
    float smokeDuration = .5f;

    Vector3 right = new Vector3(0.1f, 0, 0);
    Vector3 left = new Vector3(-0.1f, 0, 0);
    Vector3 up = new Vector3(0, 0.1f, 0);
    Vector3 down = new Vector3(0, -0.1f, 0);

    Quaternion Direction = Quaternion.Euler(0, 0, 90);

    public void LeftFoot_Vertical()
    {
        GameObject step = Instantiate(footStep, transform.position + right, transform.rotation);
        Destroy(step, stepDuration);
    }

    public void RightFoot_Vertical()
    {
        GameObject step = Instantiate(footStep, transform.position + left, transform.rotation);
        Destroy(step, stepDuration);
    }

    public void LeftFoot_Horizontal()
    {
        GameObject step = Instantiate(footStep, transform.position + up, Direction);
        Destroy(step, stepDuration);
    }

    public void RightFoot_Horizontal()
    {
        GameObject step = Instantiate(footStep, transform.position + down, Direction);
        Destroy(step, stepDuration);
    }

    public void Smoke_East()
    {
        GameObject smoke = Instantiate(runSmoke, transform.position, Quaternion.Euler(0, 0, 180));
        Destroy(smoke, smokeDuration);
    }

    public void Smoke_North()
    {
        GameObject smoke = Instantiate(runSmoke, transform.position, Quaternion.Euler(0, 0, 270));
        Destroy(smoke, smokeDuration);
    }

    public void Smoke_West()
    {
        GameObject smoke = Instantiate(runSmoke, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(smoke, smokeDuration);
    }

    public void Smoke_South()
    {
        GameObject smoke = Instantiate(runSmoke, transform.position, Quaternion.Euler(0, 0, 90));
        Destroy(smoke, smokeDuration);
    }
}
