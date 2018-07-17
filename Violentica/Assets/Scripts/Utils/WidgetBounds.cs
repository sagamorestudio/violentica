using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZO
{

    public class WidgetBounds : System.IDisposable
    {
        public Transform left_top;
        public Transform left_bottom;
        public Transform right_top;
        public Transform right_bottom;

        public float widthLocal
        {
            get
            {
                return right_top.localPosition.x - left_top.localPosition.x;
            }
        }

        public float heightLocal
        {
            get
            {
                return left_top.localPosition.y - left_bottom.localPosition.y;
            }
        }


        public WidgetBounds()
        {
            left_top = new GameObject().transform;
            left_bottom = new GameObject().transform;
            right_top = new GameObject().transform;
            right_bottom = new GameObject().transform;

            left_top.name = "(temp) BoundLeftTop";
            left_bottom.name = "(temp) BoundLeftBottom";
            right_top.name = "(temp) BoundRightTop";
            right_bottom.name = "(temp) BoundRightBottom";

            Clear();
        }

        public void Clear()
        {
            left_top.parent = null;
            right_top.parent = null;
            left_bottom.parent = null;
            right_bottom.parent = null;

            left_top.position = new Vector3(float.MaxValue, float.MinValue, 0);
            right_top.position = new Vector3(float.MinValue, float.MinValue, 0);
            left_bottom.position = new Vector3(float.MaxValue, float.MaxValue, 0);
            right_bottom.position = new Vector3(float.MinValue, float.MaxValue, 0);
        }

        public void SetParent(Component c)
        {
            var t = c.transform;
            left_top.parent = t;
            left_bottom.parent = t;
            right_top.parent = t;
            right_bottom.parent = t;
        }

        public void Calculate(Component c)
        {
            Calculate(c.gameObject);
        }

        public void Calculate(GameObject g, bool EnabledOnly = true)
        {
            Clear();
            left_top.name = "(" + g.name + ") BoundLeftTop";
            left_bottom.name = "(" + g.name + ") BoundLeftBottom";
            right_top.name = "(" + g.name + ") BoundRightTop";
            right_bottom.name = "(" + g.name + ") BoundRightBottom";
            var widgets = Util.GetComponentsInChildrenRecusively<UIWidget>(g, !EnabledOnly);
            foreach (var w in widgets)
            {
                var wt = w.transform;
                SetParent(w.transform);
                Vector3 lt = left_top.localPosition;
                Vector3 lb = left_bottom.localPosition;
                Vector3 rt = right_top.localPosition;
                Vector3 rb = right_bottom.localPosition;

                float left = 0;
                float right = 0;
                float top = 0;
                float bottom = 0;

                switch (w.pivot)
                {
                    case UIWidget.Pivot.Bottom:
                        {
                            left = -w.width / 2 * wt.localScale.x;
                            right = +w.width / 2 * wt.localScale.x;
                            top = +w.height * wt.localScale.y;
                            bottom = 0;
                        }
                        break;
                    case UIWidget.Pivot.BottomLeft:
                        {
                            left = 0;
                            right = +w.width * wt.localScale.x;
                            top = +w.height * wt.localScale.y;
                            bottom = 0;
                        }
                        break;
                    case UIWidget.Pivot.BottomRight:
                        {
                            left = 0;
                            right = +w.width * wt.localScale.x;
                            top = +w.height * wt.localScale.y;
                            bottom = 0;
                        }
                        break;
                    case UIWidget.Pivot.Center:
                        {
                            left = -w.width / 2 * wt.localScale.x;
                            right = +w.width / 2 * wt.localScale.x;
                            top = +w.height / 2 * wt.localScale.y;
                            bottom = -w.height / 2 * wt.localScale.y;
                        }
                        break;
                    case UIWidget.Pivot.Left:
                        {
                            left = 0;
                            right = +w.width * wt.localScale.x;
                            top = +w.height / 2 * wt.localScale.y;
                            bottom = -w.height / 2 * wt.localScale.y;
                        }
                        break;
                    case UIWidget.Pivot.Right:
                        {
                            left = -w.width * wt.localScale.x;
                            right = 0;
                            top = +w.height * wt.localScale.y;
                            bottom = 0;
                        }
                        break;
                    case UIWidget.Pivot.Top:
                        {
                            left = -w.width / 2 * wt.localScale.x;
                            right = +w.width / 2 * wt.localScale.x;
                            top = 0;
                            bottom = -w.height * wt.localScale.y;
                        }
                        break;
                    case UIWidget.Pivot.TopLeft:
                        {
                            left = 0;
                            right = +w.width * wt.localScale.x;
                            top = 0;
                            bottom = -w.height * wt.localScale.y;
                        }
                        break;
                    case UIWidget.Pivot.TopRight:
                        {
                            left = -w.width * wt.localScale.x;
                            right = 0;
                            top = 0;
                            bottom = -w.height * wt.localScale.y;
                        }
                        break;
                }


                lt.x = lt.x < left ? lt.x : left;
                lt.y = lt.y > top ? lt.y : top;
                lt.z = 0;

                rt.x = rt.x > right ? rt.x : right;
                rt.y = rt.y > top ? rt.y : top;
                rt.z = 0;

                lb.x = lb.x < left ? lb.x : left;
                lb.y = lb.y < bottom ? lb.y : bottom;
                lb.z = 0;

                rb.x = rb.x > right ? rb.x : right;
                rb.y = rb.y < bottom ? rb.y : bottom;
                rb.z = 0;

                left_top.localPosition = lt;
                left_bottom.localPosition = lb;
                right_top.localPosition = rt;
                right_bottom.localPosition = rb;
            }

            SetParent(g.transform);
        }

        public void Dispose()
        {
            GameObject.Destroy(left_top.gameObject);
            GameObject.Destroy(right_bottom.gameObject);
            GameObject.Destroy(left_bottom.gameObject);
            GameObject.Destroy(right_top.gameObject);
        }
    }
}

public class WidgetBounds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
