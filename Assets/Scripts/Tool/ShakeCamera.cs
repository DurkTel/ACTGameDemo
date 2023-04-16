using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ShakeCamera : MonoBehaviour
{
    public enum ShakeOrient
    {
        horizontal = 1,     //ˮƽ
        vertical,       //��ֱ
        forward,        //������
        curve,
    }

    //����
    public float mPeriod = 2;

    //ƫ������
    public float mOffPeriod = 0;

    public ShakeOrient mShakeOrient = ShakeOrient.horizontal;

    //��ʱ��
    public float mShakeTime = 10.0f;

    //��󲨷�
    public float mMaxWave = 5;

    //��С����
    public float mMinWave = 1;

    //�ܹ�����ʱ��
    private float mCurTime = 0;

    //�Ƿ�shake״̬
    public bool mIsShake = false;

    //��ʼλ��
    public Vector3 mDefaultPos;

    //�񶯷���
    public Vector3 mShakeDir;
    public Transform mCamerTrans;

    private bool mbRest = true; //����ɺ�λ���Զ���0
    private UnityAction OnFinish;

    //��ȡTransform
    public Transform GetTransform()
    {
        if (mCamerTrans == null)
        {
            mCamerTrans = gameObject.GetComponent<Transform>();
        }
        return mCamerTrans;
    }

    //����
    public void ShakeScreen(int stype, float period, float shakeTime, float maxWave, float minWave, float offPeriod = 0, bool bRest = true, UnityAction finish = null)
    {
        ShakeOrient shakeOrient = (ShakeOrient)stype;
        //������״̬        
        if (!mIsShake)
        {

            //ȷ��Transform��Ч
            if (GetTransform() == null) return;

            this.OnFinish = finish;
            mShakeOrient = shakeOrient;
            mPeriod = period;
            mShakeTime = shakeTime;
            mMaxWave = maxWave;
            mMinWave = minWave;
            mOffPeriod = offPeriod;
            mbRest = bRest;

            //����Ĭ��λ��
            mDefaultPos = transform.localPosition;

            //��ֱ���� 
            if (shakeOrient == ShakeOrient.vertical)
            {
                mShakeDir = new Vector3(0, 1, 0);
            }
            else if (shakeOrient == ShakeOrient.forward)
            {
                mShakeDir = mCamerTrans.forward;
            }
            else if (shakeOrient == ShakeOrient.horizontal)
            {
                Vector3 v1 = new Vector3(0, 1, 0);
                Vector3 v2 = mCamerTrans.forward;

                mShakeDir = Vector3.Cross(v1, v2);
                mShakeDir.Normalize();
            }

            mIsShake = true;
        }
    }

    private void OnDestroy()
    {
        OnFinish = null;
    }

    public void OnDisable()
    {
        StopShaking();
    }

    public void ShakeCameraByDir()
    {
        float factor = mCurTime / mShakeTime;
        //������
        float totalPeriod = mPeriod * Mathf.PI;

        //��ǰʱ��ֵ
        float maxValue = mMaxWave - (mMaxWave - mMinWave) * factor;

        //��ǰ����ֵ
        float radValue = mOffPeriod * Mathf.PI + factor * totalPeriod;
        float value = maxValue * Mathf.Sin(radValue);

        //��ֱ�񶯣�ֻ�̶�y����
        if (mShakeOrient == ShakeOrient.vertical)
            mCamerTrans.localPosition = new Vector3(mCamerTrans.localPosition.x, mDefaultPos.y, mCamerTrans.localPosition.z) + mShakeDir * value;
        else
            mCamerTrans.localPosition = mDefaultPos + mShakeDir * value;


        mCurTime += Time.deltaTime;
        //��������״̬��
        if (mCurTime > mShakeTime)
        {

            mIsShake = false;
            mCurTime = 0;

            mCamerTrans.localPosition = mbRest ? Vector3.zero : mDefaultPos;

            if (OnFinish != null)
            {
                OnFinish.Invoke();
                OnFinish = null;
            }
        }
    }

    public void LateUpdate()
    {
        if (mIsShake)
        {
            ShakeCameraByDir();
        }
    }

    public void StopShaking()
    {
        OnFinish = null;
        mIsShake = false;
        mCurTime = 0;
    }
}