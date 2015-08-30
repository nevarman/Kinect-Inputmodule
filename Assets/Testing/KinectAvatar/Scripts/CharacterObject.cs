using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;

public class CharacterObject : MonoBehaviour
{
    public Transform rootHips;
    public bool inverse = true;
    public float lerpValue = 10f;
    [SerializeField]
    public JointNameType[] jointNameType;
    //public Vector3 shoulderCorrection, elbowCorrection;
    //public float shoulderAxis, elbowAxis = 180f;

    public Dictionary<string, JointNameType> kinectJoints = new Dictionary<string, JointNameType>();
    public Dictionary<JointType, JointNameType> characterKJoints = new Dictionary<JointType, JointNameType>();

    void Start()
    {
        FillDict();
    }

    protected void FillDict()
    {
        for (int i = 0; i < 25; i++)
        {
            kinectJoints.Add(jointNameType[i].jointType.ToString(), jointNameType[i]);
        }
        for (int i = 0; i < jointNameType.Length; i++)
        {
            characterKJoints.Add(jointNameType[i].jointType, jointNameType[i]);
        }
    }
    void Update()
    {
        // UpdateAvatar(null);
    }

    public void UpdateAvatar(Body body)
    {
        //Debug.Log(body.HandLeftState.ToString());
        Vector3 spineBasePosition_ = BodySourceView.GetVector3FromJoint(body.Joints[JointType.SpineBase]);//showJoints[JointType.SpineBase];
        Vector3 spineBasePosition = new Vector3(spineBasePosition_.x + 0f, spineBasePosition_.y + 0.5f, spineBasePosition_.z - 0.5f);
        characterKJoints[JointType.SpineBase].boneTransform.position = spineBasePosition;

        Quaternion jOrSB = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineBase]);
        characterKJoints[JointType.SpineBase].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.SpineBase].boneTransform.rotation,
                                                                                      jOrSB * characterKJoints[JointType.SpineBase].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrSM = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineMid]);
        characterKJoints[JointType.SpineMid].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.SpineMid].boneTransform.rotation,
                                                                                      jOrSM * characterKJoints[JointType.SpineMid].globalRotation,
                                                                                      Time.deltaTime * lerpValue);// jOrSM * kinectJoints["SpineMid"].globalRotation;

        Quaternion jOrSS = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineShoulder]);
        characterKJoints[JointType.SpineShoulder].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.SpineShoulder].boneTransform.rotation,
                                                                                      jOrSS * characterKJoints[JointType.SpineShoulder].globalRotation,
                                                                                      Time.deltaTime * lerpValue);//jOrSS * kinectJoints["SpineShoulder"].globalRotation;

        Quaternion ArmsR = Quaternion.FromToRotation(Vector3.left, Vector3.up);
        Quaternion ArmsL = Quaternion.FromToRotation(Vector3.right, Vector3.up);

        Quaternion jOrSR = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.ElbowRight]);
        characterKJoints[JointType.ShoulderRight].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.ShoulderRight].boneTransform.rotation,
            jOrSR * ArmsR * characterKJoints[JointType.ShoulderRight].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrER = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.WristRight]);
        characterKJoints[JointType.ElbowRight].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.ElbowRight].boneTransform.rotation,
            jOrER * ArmsR * characterKJoints[JointType.ElbowRight].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrSL = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.ElbowLeft]);
        characterKJoints[JointType.ShoulderLeft].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.ShoulderLeft].boneTransform.rotation,
            jOrSL * ArmsL * characterKJoints[JointType.ShoulderLeft].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrEL = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.WristLeft]);
        characterKJoints[JointType.ElbowLeft].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.ElbowLeft].boneTransform.rotation,
            jOrEL * ArmsL * characterKJoints[JointType.ElbowLeft].globalRotation,
                                                                                      Time.deltaTime * lerpValue);


        Quaternion LegR = Quaternion.Euler(0, 90, 0) * Quaternion.FromToRotation(Vector3.down, Vector3.up);
        Quaternion LegL = Quaternion.Euler(0, -90, 0) * Quaternion.FromToRotation(Vector3.down, Vector3.up);


        Quaternion jOrHR = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.KneeRight]);
        characterKJoints[JointType.HipRight].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.HipRight].boneTransform.rotation,
            jOrHR * LegR * characterKJoints[JointType.HipRight].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrHL = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.KneeLeft]);
        characterKJoints[JointType.HipLeft].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.HipLeft].boneTransform.rotation,
            jOrHL * LegL * characterKJoints[JointType.HipLeft].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrKR = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.AnkleRight]);
        characterKJoints[JointType.KneeRight].boneTransform.rotation = Quaternion.Lerp(characterKJoints[JointType.KneeRight].boneTransform.rotation,
            jOrKR * LegR * characterKJoints[JointType.KneeRight].globalRotation,
                                                                                      Time.deltaTime * lerpValue);

        Quaternion jOrKL = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.AnkleLeft]);
        characterKJoints[JointType.KneeLeft].boneTransform.rotation =Quaternion.Lerp(characterKJoints[JointType.KneeLeft].boneTransform.rotation,
            jOrKL * LegL * characterKJoints[JointType.KneeLeft].globalRotation,
                                                                                      Time.deltaTime * lerpValue);


        /*
        Quaternion r4 = Quaternion.Euler(0, 0, 0);
        Quaternion r5 = Quaternion.Euler(90, 257.767f, 0);
        // Spine Base

        Vector3 pos = BodySourceView.GetVector3FromJoint(body.Joints[JointType.SpineBase]);
        Quaternion rotSpineBase = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineBase]);
        characterKJoints[JointType.SpineBase].boneTransform.localRotation = Quaternion.Lerp(
            characterKJoints[JointType.SpineBase].boneTransform.localRotation, rotSpineBase, Time.deltaTime * lerpSpeed);

        //characterKJoints[JointType.SpineBase].boneTransform.position = pos;
        ////// Mid
        //Quaternion rotSpineMid = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineMid]);
        //Quaternion initSM = characterKJoints[JointType.SpineMid].initialRotation;
        //Quaternion rotSpineMidNew = Quaternion.Inverse(rotSpineBase) * rotSpineMid;
        //characterKJoints[JointType.SpineMid].boneTransform.localRotation = Quaternion.Lerp(
        //    characterKJoints[JointType.SpineMid].boneTransform.localRotation, rotSpineMidNew, Time.deltaTime * lerpSpeed);

        //characterKJoints[JointType.SpineMid].boneTransform.position = BodySourceView.GetVector3FromJoint(body.Joints[JointType.SpineMid]);

        ////////// Spine Shoulder 
        //Quaternion rotSpineShoulder = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.SpineShoulder]);
        //Quaternion initSS = characterKJoints[JointType.SpineShoulder].initialRotation;
        //Quaternion rotSpineShoulderNew = Quaternion.Inverse(rotSpineBase * rotSpineMidNew) * rotSpineShoulder;
        //characterKJoints[JointType.SpineShoulder].boneTransform.localRotation = Quaternion.Lerp(
        //    characterKJoints[JointType.SpineShoulder].boneTransform.localRotation, rotSpineShoulderNew, Time.deltaTime * lerpSpeed);

        //characterKJoints[JointType.SpineShoulder].boneTransform.position = BodySourceView.GetVector3FromJoint(body.Joints[JointType.SpineShoulder]);

        //////// Shoulder right
        Quaternion sRight = Quaternion.FromToRotation(Vector3.left, Vector3.up);
        //Quaternion rotShoulderRight = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.ShoulderRight]);
        //Quaternion initRot = characterKJoints[JointType.ShoulderRight].boneTransform.parent.rotation;
        //////Quaternion test = rotShoulderRight * Quaternion.Inverse(rotSpineBase * rotSpineShoulderNew * rotSpineMidNew * r4 * r5) * Quaternion.Euler(shoulderCorrection);
        ////////Debug.Log(test.eulerAngles);
        //Quaternion rotShoulderRightNew =  initRot * sRight * rotShoulderRight *Quaternion.Euler(shoulderCorrection) ;
        //characterKJoints[JointType.ShoulderRight].boneTransform.rotation = Quaternion.Lerp(
        //    characterKJoints[JointType.ShoulderRight].boneTransform.rotation, rotShoulderRightNew, Time.deltaTime * lerpSpeed);
        //characterKJoints[JointType.ShoulderRight].boneTransform.position = BodySourceView.GetVector3FromJoint(body.Joints[JointType.ShoulderRight]);

        // // elbow right
        Quaternion rotElbowRight = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.ElbowRight]);
        Quaternion initSER = characterKJoints[JointType.ElbowRight].initialRotation;
        Quaternion rotElbowRightNew =  initSER * sRight * rotElbowRight * Quaternion.Euler(shoulderCorrection) ;
        characterKJoints[JointType.ShoulderRight].boneTransform.rotation = Quaternion.Lerp(
            characterKJoints[JointType.ShoulderRight].boneTransform.rotation, rotElbowRightNew, Time.deltaTime * lerpSpeed);
        //characterKJoints[JointType.ElbowRight].boneTransform.LookAt(characterKJoints[JointType.WristRight].boneTransform, Vector3.up);

        //Quaternion rot = Quaternion.FromToRotation(BodySourceView.GetVector3FromJoint(body.Joints[JointType.ElbowRight]),
        //    BodySourceView.GetVector3FromJoint(body.Joints[JointType.WristRight]));
        //characterKJoints[JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
        //    characterKJoints[JointType.ElbowRight].boneTransform.localRotation, rot, Time.deltaTime * lerpSpeed);
        //characterKJoints[JointType.ElbowRight].boneTransform.position = BodySourceView.GetVector3FromJoint(body.Joints[JointType.ElbowRight]);

        //// wrist rigth
        Quaternion rotWristRight = GetQuaternionFromJointOrientation(body.JointOrientations[JointType.WristRight]);
        Quaternion rotElbowRNew = sRight * rotWristRight; //Quaternion.Inverse(rotSpineBase * rotSpineMidNew * rotSpineShoulderNew * rotShoulderRightNew * rotElbowRightNew * r4 * r5) * rotWristRight * Quaternion.Euler(0, -90, 0);
        characterKJoints[JointType.ElbowRight].boneTransform.rotation = Quaternion.Lerp(
            characterKJoints[JointType.ElbowRight].boneTransform.rotation, rotElbowRNew, Time.deltaTime * lerpSpeed);

        //Quaternion rot2 = Quaternion.FromToRotation(BodySourceView.GetVector3FromJoint(body.Joints[JointType.WristRight]),
        //    BodySourceView.GetVector3FromJoint(body.Joints[JointType.ElbowRight]) );
        //characterKJoints[JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
        //    characterKJoints[JointType.ElbowRight].boneTransform.localRotation, rot2, Time.deltaTime * lerpSpeed);
        //characterKJoints[JointType.WristRight].boneTransform.position = BodySourceView.GetVector3FromJoint(body.Joints[JointType.WristRight]);

        //textShoulder.text = "Shoulder: " + rotShoulderRight.eulerAngles;
      //  textElbow.text = "Elbow: " + rotElbowRight.eulerAngles;
        //Quaternion comp = Quaternion.FromToRotation(new Vector3(floorPlane.position.x, floorPlane.position.y, floorPlane.position.z), Vector3.up);
        //Quaternion SpineBase = VToQ(body.JointOrientations[JointType.SpineBase].Orientation, comp);
        //Quaternion SpineMid = VToQ(body.JointOrientations[JointType.SpineMid].Orientation, SpineBase);
        //Quaternion SpineShoulder = VToQ(body.JointOrientations[JointType.SpineShoulder].Orientation, SpineMid);
        //Quaternion ShoulderLeft = VToQ(body.JointOrientations[JointType.ShoulderLeft].Orientation, comp);
        //Quaternion ShoulderRight = VToQ(body.JointOrientations[JointType.ShoulderRight].Orientation, comp);
        //Quaternion ElbowLeft = VToQ(body.JointOrientations[JointType.ElbowLeft].Orientation, comp);
        //Quaternion WristLeft = VToQ(body.JointOrientations[JointType.WristLeft].Orientation, comp);
        //Quaternion HandLeft = VToQ(body.JointOrientations[JointType.HandLeft].Orientation, comp);
        //Quaternion ElbowRight = VToQ(body.JointOrientations[JointType.ElbowRight].Orientation, comp);
        //Quaternion WristRight = VToQ(body.JointOrientations[JointType.WristRight].Orientation, comp);
        //Quaternion HandRight = VToQ(body.JointOrientations[JointType.HandRight].Orientation, comp);
        //Quaternion KneeLeft = VToQ(body.JointOrientations[JointType.KneeLeft].Orientation, comp);
        //Quaternion AnkleLeft = VToQ(body.JointOrientations[JointType.AnkleLeft].Orientation, comp);
        //Quaternion KneeRight = VToQ(body.JointOrientations[JointType.KneeRight].Orientation, comp);
        //Quaternion AnkleRight = VToQ(body.JointOrientations[JointType.AnkleRight].Orientation, comp);

        ////Quaternion q = transform.rotation;
        ////transform.rotation = Quaternion.identity;

        //characterKJoints[JointType.SpineMid].boneTransform.rotation = SpineMid;//* Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.ShoulderRight].boneTransform.rotation = ElbowRight * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.ElbowRight].boneTransform.rotation = WristRight * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        ////characterKJoints[JointType.HandRight].boneTransform.rotation = HandRight * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.ShoulderLeft].boneTransform.rotation = ElbowLeft * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.ElbowLeft].boneTransform.rotation = WristLeft * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        ////characterKJoints[JointType.HandLeft].boneTransform.rotation = HandLeft * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

        //characterKJoints[JointType.KneeRight].boneTransform.rotation = KneeRight * Quaternion.AngleAxis(180, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.AnkleRight].boneTransform.rotation = AnkleRight * Quaternion.AngleAxis(180, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.KneeLeft].boneTransform.rotation = KneeLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));
        //characterKJoints[JointType.AnkleLeft].boneTransform.rotation = AnkleLeft * Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

        //transform.rotation = q;
        */
    }

    Quaternion GetQuaternionFromJointOrientation(JointOrientation jo)
    {
        Quaternion result = new Quaternion(jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W);
        return result;
    }
    private Quaternion VToQ(Windows.Kinect.Vector4 kinectQ, Quaternion comp)
    {
        return Quaternion.Inverse(comp) * (new Quaternion(-kinectQ.X, -kinectQ.Y, kinectQ.Z, kinectQ.W));
    }
}

[System.Serializable]
public class JointNameType
{
    public string _name;
    public Windows.Kinect.JointType jointType;
    public Transform boneTransform;
    public Vector3 initialPosition = Vector3.zero;
    public Vector3 globalPosition = Vector3.zero;
    public Quaternion globalRotation = new Quaternion(0, 0, 0, 0);
    public Vector3 initialVector = Vector3.zero;

    public JointNameType(Windows.Kinect.JointType jointType)
    {
        this.jointType = jointType;
        this._name = jointType.ToString();
    }
    public JointNameType(string name)
    {
        this._name = name;
    }
    public JointNameType(Windows.Kinect.JointType jointType, Transform bone)
    {

        this.jointType = jointType;
        this._name = jointType.ToString();

        this.boneTransform = bone;
        if (bone == null) return;
        initialPosition = bone.localPosition;
        globalPosition = bone.position;
        globalRotation = bone.rotation;
        initialVector = bone.right;
    }
}


