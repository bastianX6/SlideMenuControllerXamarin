using System;
using UIKit;
using CoreGraphics;


namespace SlideMenuControllerXamarin
{

	public struct SlideMenuOptions
	{
		public static nfloat LeftViewWidth = 270.0f;
		public static nfloat LeftBezelWidth = 16.0f;
		public static nfloat ContentViewScale = 0.96f;
		public static nfloat ContentViewOpacity = 0.5f;
		public static SlideAnimation AnimationType = SlideAnimation.MakeScale;
		public static nfloat ShadowOpacity = 0.0f;
		public static nfloat ShadowRadius = 0.0f;
		public static CGSize ShadowOffset = new CGSize(0, 0);

		public static bool PanFromBezel = true;
		public static nfloat AnimationDuration = 0.4f;
		public static nfloat RightViewWidth = 270.0f;
		public static nfloat RightBezelWidth = 16.0f;
		public static bool RightPanFromBezel = true;
		public static bool HideStatusBar = true;
		public static nfloat PointOfNoReturnWidth = 44.0f;
		public static bool SimultaneousGestureRecognizers = true;
		public static UIColor OpacityViewBackgroundColor = UIColor.Black;
	}

	struct LeftPanState
	{
		public static CGRect FrameAtStartOfPan = new CGRect(0, 0, 0, 0);
		public static CGPoint StartPointOfPan = new CGPoint(0,0);
		public static bool WasOpenAtStartOfPan = false;
		public static bool WasHiddenAtStartOfPan = false;
		public static UIGestureRecognizerState LastState = UIGestureRecognizerState.Ended;
	}

	struct RightPanState
	{
		public static CGRect FrameAtStartOfPan = new CGRect(0, 0, 0, 0);
		public static CGPoint StartPointOfPan = new CGPoint(0, 0);
		public static bool WasOpenAtStartOfPan = false;
		public static bool WasHiddenAtStartOfPan = false;
		public static UIGestureRecognizerState LastState = UIGestureRecognizerState.Ended;
	}

	struct PanInfo
	{
		public SlideAction action;
		public bool ShouldBounce;
		public nfloat Velocity;
	}

	public enum SlideAnimation { 
		ContentViewDrag, MakeScale, Default
	}
}
