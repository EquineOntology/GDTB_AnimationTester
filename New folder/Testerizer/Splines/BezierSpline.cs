using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
{

    [SerializeField]
    private Vector3[] _points;

    [SerializeField]
    private BezierControlPointMode[] _modes;

    [SerializeField]
    private bool _loop;

    public bool Loop
    {
        get
        {
            return _loop;
        }
        set
        {
            _loop = value;
            if (value == true)
            {
                _modes[_modes.Length - 1] = _modes[0];
                SetControlPoint(0, _points[0]);
            }
        }
    }

    public int ControlPointCount
    {
        get
        {
            return _points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return _points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index%3 == 0)
        {
            Vector3 delta = point - _points[index];
            if (_loop)
            {
                if (index == 0)
                {
                    _points[1] += delta;
                    _points[_points.Length - 2] += delta;
                    _points[_points.Length - 1] = point;
                }
                else if (index == _points.Length - 1)
                {
                    _points[0] = point;
                    _points[1] += delta;
                    _points[index - 1] += delta;
                }
                else
                {
                    _points[index - 1] += delta;
                    _points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    _points[index - 1] += delta;
                }
                if (index + 1 < _points.Length)
                {
                    _points[index + 1] += delta;
                }
            }
        }
        _points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return _modes[(index + 1)/3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1)/3;
        _modes[modeIndex] = mode;
        if (_loop)
        {
            if (modeIndex == 0)
            {
                _modes[_modes.Length - 1] = mode;
            }
            else if (modeIndex == _modes.Length - 1)
            {
                _modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1)/3;
        BezierControlPointMode mode = _modes[modeIndex];
        if (mode == BezierControlPointMode.FREE || !_loop && (modeIndex == 0 || modeIndex == _modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex*3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = _points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= _points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= _points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = _points.Length - 2;
            }
        }

        Vector3 middle = _points[middleIndex];
        Vector3 enforcedTangent = middle - _points[fixedIndex];
        if (mode == BezierControlPointMode.ALIGNED)
        {
            enforcedTangent = enforcedTangent.normalized*Vector3.Distance(middle, _points[enforcedIndex]);
        }
        _points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount
    {
        get
        {
            return (_points.Length - 1)/3;
        }
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = _points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t)*CurveCount;
            i = (int) t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = _points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t)*CurveCount;
            i = (int) t;
            t -= i;
            i *= 3;
        }
        return
            transform.TransformPoint(Bezier.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2],
                _points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3 point = _points[_points.Length - 1];
        Array.Resize(ref _points, _points.Length + 3);
        point.x += 1f;
        _points[_points.Length - 3] = point;
        point.x += 1f;
        _points[_points.Length - 2] = point;
        point.x += 1f;
        _points[_points.Length - 1] = point;

        Array.Resize(ref _modes, _modes.Length + 1);
        _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
        EnforceMode(_points.Length - 4);

        if (_loop)
        {
            _points[_points.Length - 1] = _points[0];
            _modes[_modes.Length - 1] = _modes[0];
            EnforceMode(0);
        }
    }

    public void RemoveCurve(int index)
    {
        if (index == 0 || index == _points.Length - 1)
        {
            Debug.Log("Can't remove edge points (yet!)");
        }
        else if (index%3 == 0)
        {
            Vector3[] firstPart = new Vector3[index - 2];
            for (int i = 0; i < firstPart.Length; i++)
            {
                firstPart[i] = _points[i];
            }
            Vector3[] secondPart = new Vector3[_points.Length - (index + 1)];
            for (int i = 0; i < secondPart.Length; i++)
            {
                secondPart[i] = _points[i + (index + 1)];
            }

            Vector3[] tempPoints = new Vector3[firstPart.Length + secondPart.Length];
            for (int i = 0; i < firstPart.Length; i++)
            {
                tempPoints[i] = firstPart[i];
            }
            for (int i = 0; i < secondPart.Length; i++)
            {
                tempPoints[i + firstPart.Length] = secondPart[i];
            }
            _points = tempPoints;
        }
        else
        {
            Debug.Log("Can't remove curves by selecting the handles (yet!)");
        }
    }

    public void Reset()
    {
        _points = new Vector3[]
                  {new Vector3(1f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(3f, 0f, 0f), new Vector3(4f, 0f, 0f)};
        _modes = new BezierControlPointMode[] {BezierControlPointMode.FREE, BezierControlPointMode.FREE};
    }
}