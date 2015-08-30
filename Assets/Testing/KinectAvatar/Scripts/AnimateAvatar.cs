using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
//using Microsoft.Kinect.Face;

using System.Runtime.InteropServices;
using System;

/*using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using System.Drawing;

using Emgu.CV.VideoSurveillance;*/

//[System.Serializable]

public class AnimateAvatar  {
	
	//public Material exampleMaterial;
	public GameObject skeletonJointsBody;
	public Dictionary<string, Vector3> skeletonJoints = new Dictionary<string, Vector3> ();

	public Dictionary<Kinect.JointType,Vector3> showJoints = new Dictionary<Kinect.JointType,Vector3>();
	public Vector3 sourcePos;	
	public Quaternion spineBaseOrientaionFirst;

    [SerializeField]
    private float lerpValue = 10f;
	
	private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMapS = new Dictionary<Kinect.JointType, Kinect.JointType>()
	{
		{ Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
		{ Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
		{ Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
		{ Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
		
		{ Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
		{ Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
		{ Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
		{ Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
		
		{ Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
		{ Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
		{ Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
		{ Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
		{ Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
		{ Kinect.JointType.HandRight, Kinect.JointType.WristRight },
		{ Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
		{ Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
		{ Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
		
		{ Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
		{ Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
		{ Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
		{ Kinect.JointType.Neck, Kinect.JointType.Head },
	};
	
	private Dictionary<Kinect.JointType, Kinect.JointType> ChildMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
	{
		{ Kinect.JointType.ShoulderLeft, Kinect.JointType.ElbowLeft },
		{ Kinect.JointType.ShoulderRight, Kinect.JointType.ElbowRight },
		{ Kinect.JointType.HipLeft, Kinect.JointType.KneeLeft },
		{ Kinect.JointType.HipRight, Kinect.JointType.KneeRight },

		{ Kinect.JointType.ElbowLeft, Kinect.JointType.WristLeft },
		{ Kinect.JointType.ElbowRight, Kinect.JointType.WristRight },
		{ Kinect.JointType.KneeLeft, Kinect.JointType.AnkleLeft },
		{ Kinect.JointType.KneeRight, Kinect.JointType.AnkleRight }
	};

	private Dictionary<Kinect.JointType, Vector3> InitialMap = new Dictionary<Kinect.JointType, Vector3>()
	{
		{ Kinect.JointType.ShoulderLeft, Vector3.left },
		{ Kinect.JointType.ShoulderRight, Vector3.right},
		{ Kinect.JointType.HipLeft,Vector3.down },
		{ Kinect.JointType.HipRight, Vector3.down },

		{ Kinect.JointType.ElbowLeft, Vector3.left },
		{ Kinect.JointType.ElbowRight, Vector3.right },
		{ Kinect.JointType.KneeLeft, Vector3.down },
		{ Kinect.JointType.KneeRight, Vector3.down }
	};
	
	public void Initialize()
	{
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			skeletonJoints.Add (jt.ToString (), UnityEngine.Vector3.zero);
		}
		skeletonJointsBody = CreateBodyObjectG_ (UnityEngine.Color.green,2.0f,"activeSkeleton");
	}
	public GameObject CreateBodyObjectG_( UnityEngine.Color color,float lineWidth,string avatarName)
	{				
		skeletonJointsBody = new GameObject(avatarName);
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
		{
			GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			jointObj.transform.localScale = new Vector3(0,0,0);
            //LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            //lr.SetVertexCount(2);
            //lr.material = exampleMaterial;
            ////lr.SetWidth(0.05f, 0.05f);
            //lr.SetWidth(lineWidth, lineWidth);
            //lr.material.color = color;
			
			Vector3? targetJoint = null;
			if(_BoneMapS.ContainsKey(jt))
			{
				targetJoint = skeletonJoints[_BoneMapS[jt].ToString()];	
			}
			if(targetJoint.HasValue){
				
				//lr.SetWidth(0.05f, 0.05f);
                //lr.SetWidth(lineWidth, lineWidth);
				
				//jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				
				//jointObj.transform.localScale = new Vector3(3, 3, 3);
				
				jointObj.transform.localRotation = new Quaternion(0.0f, 1.0f, 0.0f,180.0f);
				jointObj.name = jt.ToString();
				jointObj.transform.parent = skeletonJointsBody.transform;
				
                //lr.SetPosition(0,skeletonJoints[jt.ToString()]);
                //lr.SetPosition(1,skeletonJoints[_BoneMapS[jt].ToString()]);	
				
				GameObject jointObjX = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //LineRenderer lrX = jointObjX.AddComponent<LineRenderer>();
                //lrX.SetVertexCount(2);
                //lrX.material = exampleMaterial;
                //lrX.SetWidth(0.05f, 0.05f);
                //lrX.material.color=UnityEngine.Color.red;
                jointObjX.transform.localScale = Vector3.zero;// new Vector3(0.3f, 0.3f, 0.3f);
				jointObjX.transform.localRotation = new Quaternion(0.0f, 1.0f, 0.0f,180.0f);
				jointObjX.name = jt.ToString() + "X";
				jointObjX.transform.parent = skeletonJointsBody.transform;
				
				GameObject jointObjY = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //LineRenderer lrY = jointObjY.AddComponent<LineRenderer>();
                //lrY.SetVertexCount(2);
                //lrY.material = exampleMaterial;
                //lrY.SetWidth(0.05f, 0.05f);		
                //lrY.material.color=UnityEngine.Color.green;
                jointObjY.transform.localScale = Vector3.zero;//new Vector3(0.3f, 0.3f, 0.3f);
				jointObjY.transform.localRotation = new Quaternion(0.0f, 1.0f, 0.0f,180.0f);
				jointObjY.name = jt.ToString() + "Y";
				jointObjY.transform.parent = skeletonJointsBody.transform;
				
				GameObject jointObjZ = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //LineRenderer lrZ = jointObjZ.AddComponent<LineRenderer>();
                //lrZ.SetVertexCount(2);
                //lrZ.material = exampleMaterial;
                //lrZ.SetWidth(0.1f, 0.1f);			
                //lrZ.material.color=UnityEngine.Color.blue;
                jointObjZ.transform.localScale = Vector3.zero;// new Vector3(0.3f, 0.3f, 0.3f);
				jointObjZ.transform.localRotation = new Quaternion(0.0f, 1.0f, 0.0f,180.0f);
				jointObjZ.name = jt.ToString() + "Z";
				jointObjZ.transform.parent = skeletonJointsBody.transform;
			}
		}
		return skeletonJointsBody;
	}
	
	
	public Dictionary<string, Vector3> changeSkeletonCenter(Vector3 transS)
	{
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			skeletonJoints[jt.ToString ()] = skeletonJoints[jt.ToString ()] + transS;
		}
		return skeletonJoints;
	}
	public Dictionary<string, Vector3> rotateSkeletonCenter(float angle, Vector3 center)
	{
		/*angle = angle * 3.14f / 180.0f;
		
		double [,] data = new double[3,3];		
		data[0,0] = Mathf.Cos(angle); 
		data[0,1] = 0.0; 
		data[0,2] = - Mathf.Sin(angle); 		
		data[1,0] = 0.0; 
		data[1,1] = 1.0; 
		data[1,2] = 0.0; 		
		data[2,0] = Mathf.Sin(angle); 
		data[2,1] = 0.0; 
		data[2,2] = Mathf.Cos(angle);
		Matrix<double> R = new Matrix<double> (data);
		
		
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			
			Vector3 currentVector;
			skeletonJoints[jt.ToString ()] = skeletonJoints[jt.ToString ()] - center;			
			//rotate
			currentVector.x = (float)R[0,0]* skeletonJoints[jt.ToString ()].x + (float)R[0,1]* skeletonJoints[jt.ToString ()].y + (float)R[0,2]* skeletonJoints[jt.ToString ()].z ; 
			currentVector.y = (float)R[1,0]* skeletonJoints[jt.ToString ()].x + (float)R[1,1]* skeletonJoints[jt.ToString ()].y + (float)R[1,2]* skeletonJoints[jt.ToString ()].z ; 
			currentVector.z = (float)R[2,0]* skeletonJoints[jt.ToString ()].x + (float)R[2,1]* skeletonJoints[jt.ToString ()].y + (float)R[2,2]* skeletonJoints[jt.ToString ()].z ; 
			//move back
			skeletonJoints[jt.ToString ()] = currentVector + center;	
			
		}*/
		return skeletonJoints;		
		
	}
	
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X , joint.Position.Y , joint.Position.Z ) * 10f;
	}
	
	public GameObject UpdateBodyObjectG_( /*GameObject body, Dictionary <string,Vector3> skeleton,*/ UnityEngine.Color color,Kinect.Body body)
	{				
		
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
		{			
			Vector3? sourceJoint = null;
			Vector3? targetJoint = null;			
			if(_BoneMapS.ContainsKey(jt))
			{
				targetJoint = skeletonJoints[_BoneMapS[jt].ToString()];
				sourceJoint = skeletonJoints[jt.ToString()];
				
				Transform jointObj = skeletonJointsBody.transform.FindChild(jt.ToString());
                //LineRenderer lr = jointObj.GetComponent<LineRenderer>();			


				Transform jointObjX = skeletonJointsBody.transform.FindChild(jt.ToString()+"X");
                //LineRenderer lrX = jointObjX.GetComponent<LineRenderer>();

				Transform jointObjY = skeletonJointsBody.transform.FindChild(jt.ToString()+"Y");
                //LineRenderer lrY = jointObjY.GetComponent<LineRenderer>();

				//			
				Transform jointObjZ = skeletonJointsBody.transform.FindChild(jt.ToString()+"Z");
                //LineRenderer lrZ = jointObjZ.GetComponent<LineRenderer>();

				if(targetJoint.HasValue && sourceJoint.HasValue)
				{
					jointObj.localPosition = skeletonJoints[jt.ToString()];
                    //lr.SetPosition(0, jointObj.localPosition);
                    //lr.SetPosition(1, skeletonJoints[_BoneMapS[jt].ToString()]);	
                    //lr.SetColors(color, color);



					Kinect.Vector4 orientation = body.JointOrientations[jt].Orientation;
					Quaternion qQuaternion_ = new Quaternion (orientation.X, orientation.Y, orientation.Z, orientation.W);			
					Vector3 xAxis_ = new Vector3 (1.0f,0.0f,0.0f);
					Vector3 yAxis_ = new Vector3 (0.0f,1.0f,0.0f);
					Vector3 zAxis_ = new Vector3 (0.0f,0.0f,1.0f);			
					Quaternion axisX_ = Rot (qQuaternion_, xAxis_);
					Quaternion axisY_ = Rot (qQuaternion_, yAxis_);
					Quaternion axisZ_ = Rot (qQuaternion_, zAxis_);

					Vector3 sourceJ = jointObj.localPosition;

					jointObjX.localPosition = sourceJ;
					Vector3 endPX = new Vector3(sourceJ.x+axisX_.x,sourceJ.y+axisX_.y,sourceJ.z+axisX_.z);
                    //lrX.SetPosition(0, jointObjX.localPosition);
                    //lrX.SetPosition(1,endPX);
					
					jointObjY.localPosition = sourceJ;
					Vector3 endPY = new Vector3(sourceJ.x+axisY_.x,sourceJ.y+axisY_.y,sourceJ.z+axisY_.z);
                    //lrY.SetPosition(0, jointObjY.localPosition);
                    //lrY.SetPosition(1,endPY);

					jointObjZ.localPosition = sourceJ;
					Vector3 endPZ = new Vector3(sourceJ.x+axisZ_.x,sourceJ.y+axisZ_.y,sourceJ.z+axisZ_.z);
                    //lrZ.SetPosition(0, jointObjZ.localPosition);
                    //lrZ.SetPosition(1,endPZ);


				}
				else
				{
                    //lr.enabled = false;
				}
			}
		}	
		
		return skeletonJointsBody;
		
	}

	Quaternion GetQuaternionFromJointOrientation(Kinect.JointOrientation jo)
	{
		Quaternion result = new Quaternion (jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W);
		return result;
	}


	private Quaternion Conj(Quaternion q)
	{
		Quaternion conjQ = new Quaternion (-q.x,-q.y,-q.z,q.w);
		return conjQ;
	}
	
	private Quaternion Rot(Quaternion current, Vector3 axisV)
	{
		Quaternion axis = new Quaternion(axisV.x, axisV.y,axisV.z,0.0f);
		Quaternion rotQ = current * axis * Conj (current);
		return rotQ;
	}

	public Quaternion showQuaternion; 


	Vector3 GetMainAxis(Kinect.Body body, Kinect.JointType jt)
	{
		Quaternion initialRotation = GetQuaternionFromJointOrientation (body.JointOrientations [jt]);
		Vector3 xAxis = new Vector3 (1.0f,0.0f,0.0f);
		Vector3 yAxis = new Vector3 (0.0f,1.0f,0.0f);
		Vector3 zAxis = new Vector3 (0.0f,0.0f,1.0f);			
		Quaternion axisX = Rot (initialRotation, xAxis);
		Quaternion axisY = Rot (initialRotation, yAxis);
		Quaternion axisZ = Rot (initialRotation, zAxis);
		Vector3 XRot = new Vector3(axisX.x,axisX.y,axisX.z);
		Vector3 YRot = new Vector3(axisY.x,axisY.y,axisY.z);
		Vector3 ZRot = new Vector3(axisZ.x,axisZ.y,axisZ.z);
		Quaternion currentRotX = Quaternion.FromToRotation(xAxis,XRot);
		Quaternion currentRotY = Quaternion.FromToRotation(yAxis,YRot);
		Quaternion currentRotZ = Quaternion.FromToRotation(zAxis,ZRot);

		return YRot;
	}

	Quaternion GetQuaternion(Kinect.Body body,Vector3 VBase , Kinect.JointType JointNameType, Quaternion currentInitial, Quaternion parentInitial){	
	
		Quaternion useInital = parentInitial*currentInitial;
		Quaternion revVector = Quaternion.Inverse(parentInitial) *  new Quaternion(VBase.x, VBase.y,VBase.z,0.0f) * Quaternion.Inverse(Conj(parentInitial));
		Vector3 currentY = GetMainAxis (body,JointNameType);		
		Quaternion mainQ =  Quaternion.FromToRotation (new Vector3 (revVector.x,revVector.y,revVector.z),currentY);

		return mainQ;
	}


	public void AssignKinectToCharacter(/*Dictionary<Kinect.JointType, Vector3> bJoints,Quaternion spineBaseOrientaion, */CharacterObject character, float translateP, Kinect.Body body)
	{
        //Quaternion r1 = Quaternion.Euler (0, 180, 0);
        //Quaternion r2 = Quaternion.Euler (354.306f, 0.00463f, 359.964f);
        //Quaternion r3 = Quaternion.Euler (357.08f, -1.5258f, 0.00039f);
		Quaternion r4 = Quaternion.Euler (0, 0, 0);

		Quaternion r5 = Quaternion.Euler (90, 257.767f, 0);
		//Quaternion r6 = Quaternion.Euler (358.746f,0.198f, 347.766f);

		Quaternion r5_ = Quaternion.Euler (90, 102.767f, 0);
		//Quaternion r6_ = Quaternion.Euler (358.746f,0.198f, 12.766f);
		
		//Quaternion resultAll = r1 * r2 * r3 * r4 * r5 * r6;
		//Quaternion resultER = r1 * r2 * r3 * r4 * r5 ;

        float lerpSpeed = 10f;/*
        Vector3 spineBasePosition_ = showJoints[Kinect.JointType.SpineBase];
        Vector3 spineBasePosition = new Vector3(spineBasePosition_.x + translateP, spineBasePosition_.y + 0.5f, spineBasePosition_.z - 0.5f);
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.position = spineBasePosition;
        // Spine Base
        Quaternion rotSpineBase = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineBase]);
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation, rotSpineBase, Time.deltaTime * lerpSpeed); // jOrSB;
        // Mid
        Quaternion rotSpineMid = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineMid]);
        Quaternion initSM = character.characterKJoints[Kinect.JointType.SpineMid].initialRotation;
        Quaternion rotSpineMidNew = Quaternion.Inverse( rotSpineBase) * rotSpineMid;//Quaternion.Inverse(rotSpineBase) * rotSpineMid;
        character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation, rotSpineMidNew, Time.deltaTime * lerpSpeed);
        // Shoulder 
        Quaternion rotSpineShoulder = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineShoulder]);
        Quaternion initSS = character.characterKJoints[Kinect.JointType.SpineShoulder].initialRotation;
        Quaternion rotSpineShoulderNew = Quaternion.Inverse(rotSpineBase ) * rotSpineShoulder;//Quaternion.Inverse(rotSpineBase * rotSpineMidNew) * rotSpineShoulder;
        character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation, rotSpineShoulderNew, Time.deltaTime * lerpSpeed);

        Quaternion rotShoulderRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ShoulderRight]);
        Quaternion initSR = character.characterKJoints[Kinect.JointType.ShoulderRight].initialRotation;
        Quaternion rotShoulderRightNew = Quaternion.Inverse(rotSpineBase * rotSpineMid * rotShoulderRight * r4 * r5) * rotShoulderRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation, rotShoulderRightNew, Time.deltaTime * lerpSpeed);

        //Quaternion rotElbowRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ElbowRight]);
        ////character.kinectJoints ["ShoulderRight"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0,-90,0);
        //Quaternion initSER = character.characterKJoints[Kinect.JointType.ElbowRight].initialRotation;
        //Quaternion rotElbowRightNew = Quaternion.Inverse(initSER * rotShoulderRight) * rotElbowRight;//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5) * rotElbowRight * Quaternion.Euler(0, -90, 0);
        //character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation, rotElbowRightNew, Time.deltaTime * lerpSpeed);
        */
		Vector3 spineBasePosition_ = showJoints[Kinect.JointType.SpineBase] ;		
		Vector3 spineBasePosition = new Vector3 (spineBasePosition_.x+translateP,spineBasePosition_.y + 0.5f,spineBasePosition_.z - 0.5f);
		//character.kinectJoints ["SpineBase"].boneTransform.position = spineBasePosition;
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.position = spineBasePosition;
					
        //Kinect.JointType jt = Kinect.JointType.SpineBase;
        Quaternion rotSpineBase = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineBase]);
        //character.kinectJoints [jt.ToString ()].boneTransform.localRotation =  jOrSB;
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation, rotSpineBase, Time.deltaTime * lerpSpeed); // jOrSB;
		Quaternion newr1 = rotSpineBase;


		Quaternion rotSpineMid = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.SpineMid]);
        //character.kinectJoints ["SpineMid"].boneTransform.localRotation =  Quaternion.Inverse(jOrSB) * jOrSM ;
        Quaternion newr12 = Quaternion.Inverse(rotSpineBase) * rotSpineMid;
        character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation, newr12, Time.deltaTime * lerpSpeed);
		


		Quaternion rotSpineShoulder = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.SpineShoulder]);
        //character.kinectJoints ["SpineShoulder"].boneTransform.localRotation = Quaternion.Inverse(newr1 * newr12) * jOrSS ;
        Quaternion newr13 = Quaternion.Inverse(rotSpineBase * newr12) * rotSpineShoulder;
        character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation, newr13, Time.deltaTime * lerpSpeed);// newr13;
        


		Quaternion rotElbowRight = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.ElbowRight]);
        //character.kinectJoints ["ShoulderRight"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0,-90,0);
        Quaternion newr6 = Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5) * rotElbowRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation, newr6, Time.deltaTime * lerpSpeed); //newr6;//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0, -90, 0);
        

		Quaternion rotWristRight = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.WristRight]);
        //character.kinectJoints ["ElbowRight"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5 *newr6) * jOrER * Quaternion.Euler(0,-90,0);
        Quaternion rotElbowRNew = Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5 * newr6) * rotWristRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation, rotElbowRNew, Time.deltaTime * lerpSpeed);//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5 * newr6) * rotWristRight * Quaternion.Euler(0, -90, 0);
        //Quaternion newr6 = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0,-90,0);


		Quaternion rotElbowLeft = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.ElbowLeft]);
        //character.kinectJoints ["ShoulderLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5_) * jOrSL * Quaternion.Euler(0,90,0);
        Quaternion newr6_ = Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_) * rotElbowLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation, newr6_, Time.deltaTime * lerpSpeed);//newr6_;//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5_) * jOrSL * Quaternion.Euler(0, 90, 0);
       
		
		Quaternion rotWristLeft = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.WristLeft]);
        Quaternion rotWristLeftNew = Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_ * newr6_) * rotWristLeft * Quaternion.Euler(0, 90, 0);
        //character.kinectJoints ["ElbowLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5_ *newr6_) * jOrEL * Quaternion.Euler(0,90,0);
        character.characterKJoints[Kinect.JointType.ElbowLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ElbowLeft].boneTransform.localRotation, rotWristLeftNew, Time.deltaTime * lerpSpeed);//Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_ * newr6_) * rotWristLeft * Quaternion.Euler(0, 90, 0);

		Quaternion rotKneeRight = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.KneeRight]);
        //character.kinectJoints ["HipRight"].boneTransform.localRotation = Quaternion.Inverse (newr1) * jOrHR * Quaternion.Euler(0,-90,0);
        Quaternion newr8 = Quaternion.Inverse(rotSpineBase) * rotKneeRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.HipRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.HipRight].boneTransform.localRotation, newr8, Time.deltaTime * lerpSpeed);// newr8;
        
		Quaternion rotKneeLeft = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.KneeLeft]);
        //character.kinectJoints ["HipLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1) * jOrHL * Quaternion.Euler(0,90,0);
        Quaternion newr8_ = Quaternion.Inverse(rotSpineBase) * rotKneeLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.HipLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.HipLeft].boneTransform.localRotation, newr8_, Time.deltaTime * lerpSpeed);//newr8_; //Quaternion.Inverse(newr1) * rotKneeLeft * Quaternion.Euler(0, 90, 0);
        

		Quaternion rotAnkleRight = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.AnkleRight]);
        //character.kinectJoints ["KneeRight"].boneTransform.localRotation = Quaternion.Inverse (newr1*newr8) * jOrKR * Quaternion.Euler(0,-90,0);
        Quaternion newr9 = Quaternion.Inverse(rotSpineBase * newr8) * rotAnkleRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.KneeRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.KneeRight].boneTransform.localRotation, newr9, Time.deltaTime * lerpSpeed); //newr9;
       

		Quaternion rotAnkleLeft = GetQuaternionFromJointOrientation (body.JointOrientations [Kinect.JointType.AnkleLeft]);
        //character.kinectJoints ["KneeLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1*newr8_) * jOrKL * Quaternion.Euler(0,90,0);
        Quaternion newr9_ = Quaternion.Inverse(rotSpineBase * newr8_) * rotAnkleLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.KneeLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.KneeLeft].boneTransform.localRotation, newr9_, Time.deltaTime * lerpSpeed); //newr9_;//Quaternion.Inverse(newr1 * newr8_) * jOrKL * Quaternion.Euler(0, 90, 0);
       
	}
	
	public void AssignKinectToCharacterBack(Dictionary<Kinect.JointType, Vector3> bJoints,Quaternion spineBaseOrientaion, CharacterObject character, float translateP)
	{
		
		//System.Console.Write ("normal");
		
		Vector3 spineBasePosition_ = bJoints[Kinect.JointType.SpineBase] ;		
		//Vector3 spineBasePosition = new Vector3 (spineBasePosition_.x+translateP,spineBasePosition_.y,spineBasePosition_.z-0.5f);
		Vector3 spineBasePosition = new Vector3 (spineBasePosition_.x+translateP,spineBasePosition_.y,spineBasePosition_.z);
		character.kinectJoints ["SpineBase"].boneTransform.position = spineBasePosition;
		
		Quaternion reverse = Quaternion.AngleAxis (180,Vector3.up);
		Quaternion spineBaseRotation_ = spineBaseOrientaion*reverse;
		Quaternion spineBaseRotation = new Quaternion (-spineBaseRotation_.x, -spineBaseRotation_.y, spineBaseRotation_.z, spineBaseRotation_.w);//-+-
		character.kinectJoints ["SpineBase"].boneTransform.localRotation = spineBaseRotation;
		
		
		//HIP LEFT
		Vector3 leftLegPosition = bJoints[Kinect.JointType.HipLeft];
		Vector3 leftKneePosition = bJoints[Kinect.JointType.KneeLeft];
		Vector3 leftKneeBone = leftLegPosition - leftKneePosition;
		Quaternion leftLegAngle2 =  Quaternion.FromToRotation (leftKneeBone,Vector3.up); 
		character.kinectJoints ["HipLeft"].boneTransform.localRotation =  Quaternion.Inverse(spineBaseRotation) * leftLegAngle2;
		
		//KNEE LEFT
		Vector3 leftAnklePosition = bJoints[Kinect.JointType.AnkleLeft];
		Vector3 leftAnkleBone =  leftKneePosition - leftAnklePosition;
		Quaternion leftKneeAngle2 =  Quaternion.FromToRotation (leftAnkleBone,Vector3.up);
		character.kinectJoints ["KneeLeft"].boneTransform.localRotation =Quaternion.Inverse( leftLegAngle2) * leftKneeAngle2;
		
		
		float KneeAngle = Vector3.Angle (leftKneeBone, leftAnkleBone);
		
		float ang;
		Vector3 angAxis;
		leftKneeAngle2.ToAngleAxis(out ang , out angAxis);
		//print ("angle "+ ang + "axs "+angAxis + "bone " + leftAnkleBone + "bone2" + leftKneeBone + " angle btw bones : " + KneeAngle);
		
		
		//ANKLE LEFT
		//Vector3 leftFootPosition = bJoints[Kinect.JointType.FootLeft];
		//Vector3 leftFootBone =  leftAnklePosition - leftFootPosition;
		//Quaternion leftAnkleAngle2 =  Quaternion.FromToRotation (leftFootBone,Vector3.forward);
		//character.kinectJoints ["AnkleLeft"].boneTransform.localRotation =Quaternion.Inverse( leftKneeAngle2) * leftAnkleAngle2;
		
		////////////////////////////////////
		
		//HIP RIGHT
		Vector3 rightLegPosition = bJoints[Kinect.JointType.HipRight];
		Vector3 rightKneePosition = bJoints [Kinect.JointType.KneeRight];
		Vector3 rightKneeBone = rightLegPosition - rightKneePosition;
		Quaternion rightLegAngle2 =  Quaternion.FromToRotation (rightKneeBone,Vector3.up);
		character.kinectJoints ["HipRight"].boneTransform.localRotation = Quaternion.Inverse(spineBaseRotation) * rightLegAngle2;
		
		//KNEE RIGHT
		Vector3 rightAnklePosition = bJoints[Kinect.JointType.AnkleRight];
		Vector3 rightAnkleBone =  rightKneePosition - rightAnklePosition;
		Quaternion rightKneeAngle2 =  Quaternion.FromToRotation (rightAnkleBone,Vector3.up);
		//character.kinectJoints ["KneeRight"].boneTransform.localRotation =Quaternion.Inverse( rightLegAngle2) * rightKneeAngle2;
		character.kinectJoints ["KneeRight"].boneTransform.localRotation =Quaternion.Inverse( Quaternion.Inverse(spineBaseRotation) * rightLegAngle2) * rightKneeAngle2;
		
		
		//ANKLE RIGHT
		Vector3 rightFootPosition = bJoints[Kinect.JointType.FootRight];
		Vector3 rightFootBone =  rightAnklePosition - rightFootPosition;
		Quaternion rightAnkleAngle2 =  Quaternion.FromToRotation (rightFootBone,Vector3.back);
		//character.kinectJoints ["AnkleRight"].boneTransform.localRotation = rightAnkleAngle2 * Quaternion.Inverse( Quaternion.Inverse( Quaternion.Inverse(spineBaseRotation) * rightLegAngle2) * rightKneeAngle2);
		
		////////////////////////////////////
		//Spine Mid
		
		Vector3 spineMidPosition = bJoints[Kinect.JointType.SpineMid];
		Vector3 spineShoulderPoition = bJoints[Kinect.JointType.SpineShoulder];
		Vector3 spineShoulderBone =  spineMidPosition - spineShoulderPoition;
		Quaternion spineMidAngle2 =  Quaternion.FromToRotation (spineShoulderBone,Vector3.down);
		//_characterObject.kinectJoints ["SpineMid"].boneTransform.localRotation =Quaternion.Inverse(spineBaseRotation) * spineMidAngle2;
		Quaternion spineTotal = Quaternion.Inverse(spineBaseRotation) * spineMidAngle2;
		
		
		Vector3 HeadPosition = bJoints[Kinect.JointType.Head];
		Vector3 HeadBone = spineShoulderPoition -HeadPosition;
		Quaternion headAngle =  Quaternion.FromToRotation (HeadBone,Vector3.down);
		//_characterObject.kinectJoints ["SpineShoulder"].boneTransform.localRotation =Quaternion.Inverse(spineTotal) * headAngle;
		Quaternion headTotal = Quaternion.Inverse(spineTotal) * headAngle;
		
		
		//SHOULDER LEFT
		Vector3 leftShoulderPoition = bJoints[Kinect.JointType.ShoulderLeft];
		Vector3 leftElbowPosition = bJoints[Kinect.JointType.ElbowLeft];
		Vector3 leftElbowBone =  leftShoulderPoition - leftElbowPosition;
		Quaternion leftShoulderAngle2 =  Quaternion.FromToRotation (leftElbowBone,Vector3.right);
		Quaternion left2 = new Quaternion (leftShoulderAngle2.x, -leftShoulderAngle2.y, leftShoulderAngle2.z, leftShoulderAngle2.w);
		
		character.kinectJoints ["ShoulderLeft"].boneTransform.localRotation = Quaternion.Inverse(spineBaseRotation) *left2;
		Quaternion totalLeftShoulder = left2;
		
		
		
		//SHOULDER RIGHT
		Vector3 rightShoulderPoition = bJoints[Kinect.JointType.ShoulderRight];
		Vector3 rightElbowPosition = bJoints[Kinect.JointType.ElbowRight];
		Vector3 rightElbowBone =   rightElbowPosition - rightShoulderPoition;
		Quaternion rightShoulderAngle2 =  Quaternion.FromToRotation (rightElbowBone,Vector3.right);
		Quaternion right2 = new Quaternion (rightShoulderAngle2.x, -rightShoulderAngle2.y, rightShoulderAngle2.z, rightShoulderAngle2.w);
		character.kinectJoints ["ShoulderRight"].boneTransform.localRotation = Quaternion.Inverse(spineBaseRotation) * right2;
		Quaternion totalRightShoulder = right2;
		
		
		Vector3 leftWristPosition = bJoints[Kinect.JointType.WristLeft];
		Vector3 leftWristBone =  leftElbowPosition -leftWristPosition;
		Quaternion leftElbowAngle2 =  Quaternion.FromToRotation (leftWristBone,Vector3.right);
		Quaternion leftElbow = new Quaternion (leftElbowAngle2.x, -leftElbowAngle2.y, leftElbowAngle2.z, leftElbowAngle2.w);
		character.kinectJoints ["ElbowLeft"].boneTransform.localRotation = Quaternion.Inverse( totalLeftShoulder) *leftElbow;
		
		
		Vector3 rightWristPosition = bJoints[Kinect.JointType.WristRight];
		Vector3 rightWristBone =  rightElbowPosition -rightWristPosition;
		Quaternion rightElbowAngle2 =  Quaternion.FromToRotation (rightWristBone,Vector3.left);
		Quaternion rightElbow = new Quaternion (rightElbowAngle2.x, -rightElbowAngle2.y, rightElbowAngle2.z, rightElbowAngle2.w);
		character.kinectJoints ["ElbowRight"].boneTransform.localRotation = Quaternion.Inverse( totalRightShoulder) *rightElbow;
		Quaternion totalWrist = Quaternion.Inverse( totalRightShoulder)*rightElbowAngle2;
		
		
		Vector3 rightHandTipPosition = bJoints[Kinect.JointType.HandTipRight];
		Vector3 rightHandTipBone =  rightWristBone -rightHandTipPosition;
		Quaternion rightHandAngle2 =  Quaternion.FromToRotation (rightHandTipBone,Vector3.left);
		//_characterObject.kinectJoints ["HandRight"].boneTransform.localRotation =  Quaternion.Inverse ( rightHandAngle2)*rightHandAngle;*/
		
	}

    public void UpdateAvatarBody(CharacterObject character, Kinect.Body body)
    {

        //Quaternion r1 = Quaternion.Euler (0, 180, 0);
        //Quaternion r2 = Quaternion.Euler (354.306f, 0.00463f, 359.964f);
        //Quaternion r3 = Quaternion.Euler (357.08f, -1.5258f, 0.00039f);
        Quaternion r4 = Quaternion.Euler(0, 0, 0);

        Quaternion r5 = Quaternion.Euler(90, 257.767f, 0);
        //Quaternion r6 = Quaternion.Euler (358.746f,0.198f, 347.766f);

        Quaternion r5_ = Quaternion.Euler(90, 102.767f, 0);
        //Quaternion r6_ = Quaternion.Euler (358.746f,0.198f, 12.766f);

        //Quaternion resultAll = r1 * r2 * r3 * r4 * r5 * r6;
        //Quaternion resultER = r1 * r2 * r3 * r4 * r5 ;

        //Vector3 spineBasePosition_ = showJoints[Kinect.JointType.SpineBase];
        //Vector3 spineBasePosition = new Vector3(spineBasePosition_.x , spineBasePosition_.y , spineBasePosition_.z);
        //character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.position = spineBasePosition;
        // Spine Base
        //Quaternion rotSpineBase = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineBase]);
        //character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation, rotSpineBase, Time.deltaTime * lerpSpeed); // jOrSB;
        //// Mid
        //Quaternion rotSpineMid = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineMid]);
        //Quaternion initSM = character.characterKJoints[Kinect.JointType.SpineMid].initialRotation;
        //Quaternion rotSpineMidNew = Quaternion.Inverse(rotSpineBase) * rotSpineMid;//Quaternion.Inverse(rotSpineBase) * rotSpineMid;
        //character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation, rotSpineMidNew, Time.deltaTime * lerpSpeed);
        ////// Spine Shoulder 
        //Quaternion rotSpineShoulder = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineShoulder]);
        //Quaternion initSS = character.characterKJoints[Kinect.JointType.SpineShoulder].initialRotation;
        //Quaternion rotSpineShoulderNew = Quaternion.Inverse(rotSpineBase * rotSpineMidNew) * rotSpineShoulder;//Quaternion.Inverse(rotSpineBase * rotSpineMidNew) * rotSpineShoulder;
        //character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation, rotSpineShoulderNew, Time.deltaTime * lerpSpeed);
        //// Shoulder right
        //Quaternion rotShoulderRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ShoulderRight]);
        //Quaternion initSR = character.characterKJoints[Kinect.JointType.ShoulderRight].initialRotation;
        //Quaternion rotShoulderRightNew = Quaternion.Inverse(rotSpineBase * rotSpineMidNew * rotSpineShoulderNew * r4 * r5) * rotShoulderRight * Quaternion.Euler(0, -90, 0);
        //character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation, rotShoulderRightNew, Time.deltaTime * lerpSpeed);
        //// elbow right
        //Quaternion rotElbowRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ElbowRight]);
        //Quaternion initSER = character.characterKJoints[Kinect.JointType.ElbowRight].initialRotation;
        //Quaternion rotElbowRightNew = Quaternion.Inverse(rotSpineBase * rotSpineMidNew * rotSpineShoulderNew * rotShoulderRightNew * r4 * r5) * rotElbowRight * Quaternion.Euler(0, -90, 0);
        //character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation, rotElbowRightNew, Time.deltaTime * lerpSpeed);
        ////// wrist rigth
        //Quaternion rotWristRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.WristRight]);
        //Quaternion rotElbowRNew = Quaternion.Inverse(rotSpineBase * rotSpineMidNew * rotSpineShoulderNew * rotShoulderRightNew * rotElbowRightNew * r4 * r5) * rotWristRight * Quaternion.Euler(0, -90, 0);
        //character.characterKJoints[Kinect.JointType.WristRight].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.WristRight].boneTransform.localRotation, rotElbowRNew, Time.deltaTime * lerpSpeed);


        //Quaternion rotElbowLeft = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ElbowLeft]);
        ////character.kinectJoints ["ShoulderLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5_) * jOrSL * Quaternion.Euler(0,90,0);
        //Quaternion newr6_ = Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_) * rotElbowLeft * Quaternion.Euler(0, 90, 0);
        //character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation = Quaternion.Lerp(
        //    character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation, newr6_, Time.deltaTime * lerpSpeed);

        /*
        Vector3 spineBasePosition_ = showJoints[Kinect.JointType.SpineBase];
        Vector3 spineBasePosition = new Vector3(spineBasePosition_.x , spineBasePosition_.y , spineBasePosition_.z );
        //character.kinectJoints ["SpineBase"].boneTransform.position = spineBasePosition;
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.position = spineBasePosition;

        //Kinect.JointType jt = Kinect.JointType.SpineBase;
        Quaternion rotSpineBase = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineBase]);
        //character.kinectJoints [jt.ToString ()].boneTransform.localRotation =  jOrSB;
        character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineBase].boneTransform.localRotation, rotSpineBase, Time.deltaTime * lerpSpeed); // jOrSB;
        Quaternion newr1 = rotSpineBase;


        Quaternion rotSpineMid = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineMid]);
        //character.kinectJoints ["SpineMid"].boneTransform.localRotation =  Quaternion.Inverse(jOrSB) * jOrSM ;
        Quaternion newr12 = Quaternion.Inverse(rotSpineBase) * rotSpineMid;
        character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineMid].boneTransform.localRotation, newr12, Time.deltaTime * lerpSpeed);



        Quaternion rotSpineShoulder = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.SpineShoulder]);
        //character.kinectJoints ["SpineShoulder"].boneTransform.localRotation = Quaternion.Inverse(newr1 * newr12) * jOrSS ;
        Quaternion newr13 = Quaternion.Inverse(rotSpineBase * newr12) * rotSpineShoulder;
        character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.SpineShoulder].boneTransform.localRotation, newr13, Time.deltaTime * lerpSpeed);// newr13;



        Quaternion rotElbowRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ElbowRight]);
        //character.kinectJoints ["ShoulderRight"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0,-90,0);
        Quaternion newr6 = Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5) * rotElbowRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ShoulderRight].boneTransform.localRotation, newr6, Time.deltaTime * lerpSpeed); //newr6;//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0, -90, 0);


        Quaternion rotWristRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.WristRight]);
        //character.kinectJoints ["ElbowRight"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5 *newr6) * jOrER * Quaternion.Euler(0,-90,0);
        Quaternion rotElbowRNew = Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5 * newr6) * rotWristRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ElbowRight].boneTransform.localRotation, rotElbowRNew, Time.deltaTime * lerpSpeed);//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5 * newr6) * rotWristRight * Quaternion.Euler(0, -90, 0);
        //Quaternion newr6 = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5) * jOrSR * Quaternion.Euler(0,-90,0);


        Quaternion rotElbowLeft = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.ElbowLeft]);
        //character.kinectJoints ["ShoulderLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5_) * jOrSL * Quaternion.Euler(0,90,0);
        Quaternion newr6_ = Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_) * rotElbowLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ShoulderLeft].boneTransform.localRotation, newr6_, Time.deltaTime * lerpSpeed);//newr6_;//Quaternion.Inverse(rotSpineBase * newr12 * newr13 * r4 * r5_) * jOrSL * Quaternion.Euler(0, 90, 0);


        Quaternion rotWristLeft = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.WristLeft]);
        Quaternion rotWristLeftNew = Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_ * newr6_) * rotWristLeft * Quaternion.Euler(0, 90, 0);
        //character.kinectJoints ["ElbowLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1 * newr12 * newr13 * r4 * r5_ *newr6_) * jOrEL * Quaternion.Euler(0,90,0);
        character.characterKJoints[Kinect.JointType.ElbowLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.ElbowLeft].boneTransform.localRotation, rotWristLeftNew, Time.deltaTime * lerpSpeed);//Quaternion.Inverse(newr1 * newr12 * newr13 * r4 * r5_ * newr6_) * rotWristLeft * Quaternion.Euler(0, 90, 0);

        Quaternion rotKneeRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.KneeRight]);
        //character.kinectJoints ["HipRight"].boneTransform.localRotation = Quaternion.Inverse (newr1) * jOrHR * Quaternion.Euler(0,-90,0);
        Quaternion newr8 = Quaternion.Inverse(rotSpineBase) * rotKneeRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.HipRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.HipRight].boneTransform.localRotation, newr8, Time.deltaTime * lerpSpeed);// newr8;

        Quaternion rotKneeLeft = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.KneeLeft]);
        //character.kinectJoints ["HipLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1) * jOrHL * Quaternion.Euler(0,90,0);
        Quaternion newr8_ = Quaternion.Inverse(rotSpineBase) * rotKneeLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.HipLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.HipLeft].boneTransform.localRotation, newr8_, Time.deltaTime * lerpSpeed);//newr8_; //Quaternion.Inverse(newr1) * rotKneeLeft * Quaternion.Euler(0, 90, 0);


        Quaternion rotAnkleRight = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.AnkleRight]);
        //character.kinectJoints ["KneeRight"].boneTransform.localRotation = Quaternion.Inverse (newr1*newr8) * jOrKR * Quaternion.Euler(0,-90,0);
        Quaternion newr9 = Quaternion.Inverse(rotSpineBase * newr8) * rotAnkleRight * Quaternion.Euler(0, -90, 0);
        character.characterKJoints[Kinect.JointType.KneeRight].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.KneeRight].boneTransform.localRotation, newr9, Time.deltaTime * lerpSpeed); //newr9;


        Quaternion rotAnkleLeft = GetQuaternionFromJointOrientation(body.JointOrientations[Kinect.JointType.AnkleLeft]);
        //character.kinectJoints ["KneeLeft"].boneTransform.localRotation = Quaternion.Inverse (newr1*newr8_) * jOrKL * Quaternion.Euler(0,90,0);
        Quaternion newr9_ = Quaternion.Inverse(rotSpineBase * newr8_) * rotAnkleLeft * Quaternion.Euler(0, 90, 0);
        character.characterKJoints[Kinect.JointType.KneeLeft].boneTransform.localRotation = Quaternion.Lerp(
            character.characterKJoints[Kinect.JointType.KneeLeft].boneTransform.localRotation, newr9_, Time.deltaTime * lerpSpeed); //newr9_;//Quaternion.Inverse(newr1 * newr8_) * jOrKL * Quaternion.Euler(0, 90, 0);
       */
    }
}
