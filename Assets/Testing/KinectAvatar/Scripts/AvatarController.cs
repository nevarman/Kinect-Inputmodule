using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
//using Microsoft.Kinect.Face;

/*using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System;
using System.Drawing;
*/

public class AvatarController : MonoBehaviour {

	private BodySourceManager _BodyManager;
    private CharacterObject[] _characterObjects;
	private AnimateAvatar _avatar = new AnimateAvatar();
    //public bool currentBody;
	private bool skeletonOnce = true;


	void Start()
	{
        _BodyManager = FindObjectOfType<BodySourceManager>();
        _characterObjects = FindObjectsOfType<CharacterObject>();
	}


	public void GetCurrentBody(Kinect.Body body ){
		
		int i = 0;
		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			
			Kinect.Joint? current =  null;
			current = body.Joints [jt];
			
			if(current.HasValue){
				Vector3 currentVector = 10.0f * new Vector3(body.Joints [jt].Position.X,  body.Joints [jt].Position.Y, body.Joints [jt].Position.Z);
				_avatar.skeletonJoints [jt.ToString()] = currentVector - new Vector3(0,0,18);
			}
			i++;
		}
		_avatar.sourcePos = _avatar.skeletonJoints["SpineBase"];
		
		float x = body.JointOrientations [Kinect.JointType.SpineBase].Orientation.X;
		float y = body.JointOrientations [Kinect.JointType.SpineBase].Orientation.Y;
		float z = body.JointOrientations [Kinect.JointType.SpineBase].Orientation.Z;
		float w = body.JointOrientations [Kinect.JointType.SpineBase].Orientation.W;		
		_avatar.spineBaseOrientaionFirst = new Quaternion (x, y, z, w);

		for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
			_avatar.showJoints [jt] = _avatar.skeletonJoints [jt.ToString ()];
		}
		
	}

	public void Update () 
	{
		if (_BodyManager == null)
		{
			return;
		}
		Kinect.Body[] data = _BodyManager.GetData();
		if (data == null)
		{
			return;
		}
		
		List<ulong> trackedIds = new List<ulong>();
		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{
				trackedIds.Add (body.TrackingId);
			}
		}

		foreach(var body in data)
		{
			if (body == null)
			{
				continue;
			}
			
			if(body.IsTracked)
			{
				GetCurrentBody( body );

				//iskelet hareketini
				if (skeletonOnce) {
					_avatar.CreateBodyObjectG_ (UnityEngine.Color.red,0.2f,"skeletonFirst");
					skeletonOnce = false;
				}
				_avatar.UpdateBodyObjectG_ (UnityEngine.Color.gray,body);
				//avatar ve kıyafet avatarının hareketi
                for (int i = 0; i < _characterObjects.Length; i++)
                {
                    _characterObjects[i].UpdateAvatar(body);
                   // KinectUIInputModule.instance.TrackBody(body);
                    KinectInputModule.instance.TrackBody(body);
                    //_avatar.AssignKinectToCharacter(_characterObjects[i], 0.0f, body);
                    //_avatar.UpdateAvatarBody(_characterObjects[i], body);
                    //Debug.Log(_characterObjects[i].GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh.vertices[0]);
                    //Debug.Log(BodySourceView.GetVector3FromJoint(body.Joints[Kinect.JointType.ElbowLeft]));
                }
			}
		}
	}

}
