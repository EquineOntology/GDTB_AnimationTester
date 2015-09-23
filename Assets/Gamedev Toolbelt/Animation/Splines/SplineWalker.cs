using UnityEngine;

public class SplineWalker : MonoBehaviour
{

    public BezierSpline Spline;

    public float Duration;

    public bool LookForward = true;
	public bool LookAtTarget = false;
	public Transform Target;

    public SplineWalkerMode Mode;
    public bool Active = false;

    private float _progress;
    private bool _goingForward = true;


    private void Update()
    {
        if (_goingForward)
        {
            _progress += Time.deltaTime/Duration;
            if (_progress > 1f)
            {
                if (Mode == SplineWalkerMode.ONCE)
                {
                    _progress = 1f;
                }
                else if (Mode == SplineWalkerMode.LOOP)
                {
                    _progress -= 1f;
                }
                else
                {
                    _progress = 2f - _progress;
                    _goingForward = false;
                }
            }
        }
        else
        {
            _progress -= Time.deltaTime/Duration;
            if (_progress < 0f)
            {
                _progress = -_progress;
                _goingForward = true;
            }
        }

        Vector3 position = Spline.GetPoint(_progress);
        transform.localPosition = position;
        if (LookForward) 
		{
			transform.LookAt(position + Spline.GetDirection (_progress));
		}
		else if (LookAtTarget) 
		{
			transform.LookAt(Target.position);
		}
    }
}