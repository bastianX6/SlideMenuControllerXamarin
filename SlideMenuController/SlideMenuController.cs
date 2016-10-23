using System;
using UIKit;
using CoreGraphics;

namespace SlideMenuControllerXamarin
{
	public partial class SlideMenuController : UIViewController
	{

		//change delegate mehtods for actions

		public Action LeftWillOpen;
		public Action LeftDidOpen;
		public Action LeftWillClose;
		public Action LeftDidClose;
		public Action RightWillOpen;
		public Action RightDidOpen;
		public Action RightWillClose;
		public Action RightDidClose;


		public UIView OpacityView = new UIView();
		public UIView MainContainerView = new UIView();
		public UIView LeftContainerView = new UIView();
		public UIView RightContainerView = new UIView();

		public UIViewController MainViewController;
		public UIViewController LeftViewController;
		public UIViewController RightViewController;

		public UIPanGestureRecognizer LeftPanGesture;
		public UIPanGestureRecognizer RightPanGesture;

		public UITapGestureRecognizer LeftTapGesture;
		public UITapGestureRecognizer RightTapGesture;


		public SlideMenuController(IntPtr handle) : base(handle)
		{
		}

		public SlideMenuController() : base("SlideMenuController", null)
		{
		}

		public SlideMenuController(UIViewController mainViewcontroller, UIViewController leftViewController,
								   UIViewController rightViewController)
		{
			MainViewController = mainViewcontroller;
			LeftViewController = leftViewController;
			RightViewController = rightViewController;
			InitView();
		}



		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			EdgesForExtendedLayout = new UIRectEdge();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (MainViewController != null)
			{
				MainViewController.ViewWillAppear(animated);
			}
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			InitView();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			if (MainViewController != null)
			{
				return MainViewController.GetSupportedInterfaceOrientations();
			}

			return UIInterfaceOrientationMask.All;
		}

		public override bool ShouldAutorotate()
		{
			if (MainViewController != null)
			{
				return MainViewController.ShouldAutorotate();
			}

			return false;
		}

		public override void ViewWillLayoutSubviews()
		{
			SetUpViewController(MainContainerView, MainViewController);
			SetUpViewController(LeftContainerView, LeftViewController);
			SetUpViewController(RightContainerView, RightViewController);
		}

		public void InitView()
		{
			MainContainerView = new UIView(View.Bounds);
			MainContainerView.BackgroundColor = UIColor.Clear;
			MainContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			View.InsertSubview(MainContainerView, 0);


			CGRect OpacityFrame = View.Bounds;
			nfloat OpacityOffset = 0;

			OpacityFrame.Y = OpacityFrame.Y + OpacityOffset;
			OpacityFrame.Height = OpacityFrame.Height - OpacityOffset;
			OpacityView = new UIView(OpacityFrame);
			OpacityView.BackgroundColor = SlideMenuOptions.OpacityViewBackgroundColor;
			OpacityView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			OpacityView.Layer.Opacity = 0.0f;
			View.InsertSubview(OpacityView, 1);

			if (LeftViewController != null)
			{
				CGRect LeftFrame = View.Bounds;
				LeftFrame.Width = SlideMenuOptions.LeftViewWidth;
				LeftFrame.X = LeftMinOrigin();
				nfloat LeftOffset = 0;
				LeftFrame.Y = LeftFrame.Y + LeftOffset;
				LeftFrame.Height = LeftFrame.Height - LeftOffset;
				LeftContainerView = new UIView(LeftFrame);
				LeftContainerView.BackgroundColor = UIColor.Clear;
				LeftContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				View.InsertSubview(LeftContainerView, 2);
				AddLeftGestures();
			}

			if (RightViewController != null)
			{
				CGRect RightFrame = View.Bounds;
				RightFrame.Width = SlideMenuOptions.RightViewWidth;
				RightFrame.X = RightMinOrigin();
				nfloat RightOffset = 0;
				RightFrame.Y = RightFrame.Y + RightOffset;
				RightFrame.Height = RightFrame.Height - RightOffset;
				RightContainerView = new UIView(RightFrame);
				RightContainerView.BackgroundColor = UIColor.Clear;
				RightContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				View.InsertSubview(RightContainerView, 3);
				AddRightGestures();
			}
		}

		public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize(toSize, coordinator);

			MainContainerView.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
			LeftContainerView.Hidden = true;
			RightContainerView.Hidden = true;

			coordinator.AnimateAlongsideTransition((obj) => { }, (IUIViewControllerTransitionCoordinatorContext obj) =>
			{
				CloseLeftNonAnimation();
				CloseRightNonAnimation();
				LeftContainerView.Hidden = false;
				RightContainerView.Hidden = false;

				if (LeftPanGesture != null && LeftTapGesture != null)
				{
					RemoveLeftGestures();
					AddLeftGestures();
				}

				if (RightPanGesture != null && RightTapGesture != null)
				{
					RemoveRightGestures();
					AddRightGestures();

				}
			});
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.

		}

		// open and close controller

		public void OpenLeft()
		{
			if (LeftViewController != null)
			{
				if (LeftWillOpen != null)
				{
					LeftWillOpen();
				}

				SetOpenWindowLevel();
				LeftViewController.BeginAppearanceTransition(IsLeftHidden(), true);
				OpenLeftWithVelocity(0.0f);
				Track(TrackAction.LeftTapOpen);

			}
		}

		public void CloseLeft()
		{
			if (LeftViewController != null)
			{
				if (LeftWillClose != null)
				{
					LeftWillClose();
				}

				LeftViewController.BeginAppearanceTransition(IsLeftHidden(), true);
				CloseLeftWithVelocity(0.0f);
				SetOpenWindowLevel();

			}
		}

		public void OpenRight()
		{
			if (RightViewController != null)
			{
				if (RightWillOpen != null)
				{
					RightWillOpen();
				}

				SetOpenWindowLevel();
				RightViewController.BeginAppearanceTransition(IsRightHidden(), true);
				OpenRightWithVelocity(0.0f);
				Track(TrackAction.RightTapOpen);
			}
		}

		public void CloseRight()
		{
			if (RightViewController != null)
			{
				if (RightWillClose != null)
				{
					RightWillClose();
				}

				RightViewController.BeginAppearanceTransition(IsRightHidden(), true);
				CloseRightWithVelocity(0.0f);
				SetOpenWindowLevel();

			}
		}


		// add and remove gestures  -----------------------------------------------------------------

		public void AddLeftGestures()
		{
			if (LeftViewController != null)
			{
				if (LeftPanGesture == null)
				{
					LeftPanGesture = new UIPanGestureRecognizer();
					LeftPanGesture.AddTarget(() => HandleLeftPanGesture(LeftPanGesture));
					LeftPanGesture.ShouldReceiveTouch += ShoudReceiveTouch;
					LeftPanGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneously;
					View.AddGestureRecognizer(LeftPanGesture);
				}

				if (LeftTapGesture == null)
				{
					LeftTapGesture = new UITapGestureRecognizer();
					LeftTapGesture.AddTarget(() => ToggleLeft());
					LeftTapGesture.ShouldReceiveTouch += ShoudReceiveTouch;
					LeftTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneously;
					View.AddGestureRecognizer(LeftTapGesture);
				}
			}
		}

		public void AddRightGestures()
		{
			if (RightViewController != null)
			{
				if (RightPanGesture == null)
				{
					RightPanGesture = new UIPanGestureRecognizer();
					RightPanGesture.AddTarget(() => HandleRightPanGesture(RightPanGesture));
					RightPanGesture.ShouldReceiveTouch += ShoudReceiveTouch;
					RightPanGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneously;
					View.AddGestureRecognizer(RightPanGesture);
				}

				if (RightTapGesture == null)
				{
					RightTapGesture = new UITapGestureRecognizer();
					RightTapGesture.AddTarget(() => ToggleRight());
					RightTapGesture.ShouldReceiveTouch += ShoudReceiveTouch;
					RightTapGesture.ShouldRecognizeSimultaneously += ShouldRecognizeSimultaneously;
					View.AddGestureRecognizer(RightTapGesture);
				}
			}
		}

		public void RemoveLeftGestures()
		{
			if (LeftPanGesture != null)
			{
				View.RemoveGestureRecognizer(LeftPanGesture);
				LeftPanGesture = null;
			}

			if (LeftTapGesture != null)
			{
				View.RemoveGestureRecognizer(LeftTapGesture);
				LeftTapGesture = null;
			}
		}

		public void RemoveRightGestures()
		{
			if (RightPanGesture != null)
			{
				View.RemoveGestureRecognizer(RightPanGesture);
				RightPanGesture = null;
			}

			if (RightTapGesture != null)
			{
				View.RemoveGestureRecognizer(RightTapGesture);
				RightTapGesture = null;
			}
		}



		nfloat LeftMinOrigin()
		{
			return -SlideMenuOptions.LeftViewWidth;
		}

		nfloat RightMinOrigin()
		{
			return View.Bounds.Width;
		}

		public virtual bool IsTargetViewController()
		{
			return true;
		}

		public virtual void Track(TrackAction tracAction) { }

		// Handle gestures -----------------------------------------------------------------


		public void HandleLeftPanGesture(UIPanGestureRecognizer panGesture)
		{
			if (!IsTargetViewController())
			{
				return;
			}

			if (IsRightOpen())
			{
				return;
			}


			switch (panGesture.State)
			{
				case UIGestureRecognizerState.Began:

					if (LeftPanState.LastState != UIGestureRecognizerState.Ended &&
					   LeftPanState.LastState != UIGestureRecognizerState.Cancelled &&
					   LeftPanState.LastState != UIGestureRecognizerState.Failed)
					{
						return;
					}

					if (IsLeftHidden() && LeftWillOpen != null)
					{
						LeftWillOpen();
					}
					else if (LeftWillClose != null)
					{
						LeftWillClose();
					}

					LeftPanState.FrameAtStartOfPan = LeftContainerView.Frame;
					LeftPanState.StartPointOfPan = panGesture.LocationInView(View);
					LeftPanState.WasOpenAtStartOfPan = IsLeftOpen();
					LeftPanState.WasHiddenAtStartOfPan = IsLeftHidden();

					if (LeftViewController != null)
					{
						LeftViewController.BeginAppearanceTransition(LeftPanState.WasHiddenAtStartOfPan, true);
					}

					AddShadowToView(LeftContainerView);
					SetOpenWindowLevel();

					break;
				case UIGestureRecognizerState.Changed:

					if (LeftPanState.LastState != UIGestureRecognizerState.Began &&
					    LeftPanState.LastState != UIGestureRecognizerState.Changed)
					{
						return;
					}

					CGPoint Translation = panGesture.TranslationInView(panGesture.View);
					LeftContainerView.Frame = ApplyLeftTranslation(Translation, LeftPanState.FrameAtStartOfPan);
					ApplyLeftOpacity();
					ApplyLeftContentViewScale();

					break;
				case UIGestureRecognizerState.Ended:
				case UIGestureRecognizerState.Cancelled:

					if (LeftPanState.LastState != UIGestureRecognizerState.Changed)
					{
						SetCloseWindowLevel();
						return;
					}

					CGPoint Velocity = panGesture.VelocityInView(panGesture.View);
					PanInfo panInfo = PanLeftResultInfoForVelocity(Velocity);

					if (panInfo.action == SlideAction.Open)
					{
						if (!LeftPanState.WasHiddenAtStartOfPan && LeftViewController != null)
						{
							LeftViewController.BeginAppearanceTransition(true, true);
						}
						OpenLeftWithVelocity(panInfo.Velocity);
						Track(TrackAction.LeftFlickOpen);
					}
					else
					{
						if (!LeftPanState.WasHiddenAtStartOfPan && LeftViewController != null)
						{
							LeftViewController.BeginAppearanceTransition(false, true);
						}
						CloseLeftWithVelocity(panInfo.Velocity);
						SetCloseWindowLevel();
						Track(TrackAction.LeftFlickClose);
					}

					break;
				case UIGestureRecognizerState.Failed:
				case UIGestureRecognizerState.Possible:
					break;
				default:
					break;

			}

			LeftPanState.LastState = panGesture.State;
		}

		public void ToggleLeft()
		{
			if (IsLeftOpen()) 
			{
				CloseLeft();
				SetCloseWindowLevel();
				Track(TrackAction.LeftTapClose);

			}
			else {
				OpenLeft();
  
			}
		}

		public void HandleRightPanGesture(UIPanGestureRecognizer panGesture)
		{
			if (!IsTargetViewController())
			{
				return;
			}

			if (IsLeftOpen())
			{
				return;
			}


			switch (panGesture.State)
			{
				case UIGestureRecognizerState.Began:

					if (RightPanState.LastState != UIGestureRecognizerState.Ended &&
					   RightPanState.LastState != UIGestureRecognizerState.Cancelled &&
					   RightPanState.LastState != UIGestureRecognizerState.Failed)
					{
						return;
					}

					if (IsRightHidden() && RightWillOpen != null)
					{
						RightWillOpen();
					}
					else if (RightWillClose != null)
					{
						RightWillClose();
					}

					RightPanState.FrameAtStartOfPan = RightContainerView.Frame;
					RightPanState.StartPointOfPan = panGesture.LocationInView(View);
					RightPanState.WasOpenAtStartOfPan = IsRightOpen();
					RightPanState.WasHiddenAtStartOfPan = IsRightHidden();

					if (RightViewController != null)
					{
						RightViewController.BeginAppearanceTransition(RightPanState.WasHiddenAtStartOfPan, true);
					}

					AddShadowToView(RightContainerView);
					SetOpenWindowLevel();

					break;
				case UIGestureRecognizerState.Changed:

					if (RightPanState.LastState != UIGestureRecognizerState.Began &&
						RightPanState.LastState != UIGestureRecognizerState.Changed)
					{
						return;
					}

					CGPoint Translation = panGesture.TranslationInView(panGesture.View);
					RightContainerView.Frame = ApplyRightTranslation(Translation, RightPanState.FrameAtStartOfPan);
					ApplyRightOpacity();
					ApplyRightContentViewScale();

					break;
				case UIGestureRecognizerState.Ended:
				case UIGestureRecognizerState.Cancelled:

					if (RightPanState.LastState != UIGestureRecognizerState.Changed)
					{
						SetCloseWindowLevel();
						return;
					}

					CGPoint Velocity = panGesture.VelocityInView(panGesture.View);
					PanInfo panInfo = PanRightResultInfoForVelocity(Velocity);

					if (panInfo.action == SlideAction.Open)
					{
						if (!RightPanState.WasHiddenAtStartOfPan && RightViewController != null)
						{
							RightViewController.BeginAppearanceTransition(true, true);
						}
						OpenRightWithVelocity(panInfo.Velocity);
						Track(TrackAction.RightFlickOpen);
					}
					else
					{
						if (!RightPanState.WasHiddenAtStartOfPan && RightViewController != null)
						{
							RightViewController.BeginAppearanceTransition(false, true);
						}
						CloseRightWithVelocity(panInfo.Velocity);
						SetCloseWindowLevel();
						Track(TrackAction.RightFlickClose);
					}

					break;
				case UIGestureRecognizerState.Failed:
				case UIGestureRecognizerState.Possible:
					break;

			}

			RightPanState.LastState = panGesture.State;
		}

		public void ToggleRight()
		{
			if (IsRightOpen()) 
			{
				CloseRight();
				SetCloseWindowLevel();
				Track(TrackAction.RightTapClose);

			}
			else 
			{
				OpenRight();
  
			}
		}

		// If left/right open/hidden -----------------------------------------------------------------

		public bool IsLeftOpen()
		{
			return LeftViewController != null && LeftContainerView.Frame.X == 0;
		}

		public bool IsLeftHidden()
		{
			return LeftContainerView.Frame.X <= LeftMinOrigin();
		}

		public bool IsRightOpen()
		{
			return RightViewController != null && 
				RightContainerView.Frame.X == View.Bounds.Width - RightContainerView.Frame.Size.Width;
		}

		public bool IsRightHidden()
		{
			return RightContainerView.Frame.X >= View.Bounds.Width;
		}

		// Open/Close Left/Right with velocity -----------------------------------------------------------------

		public void OpenLeftWithVelocity(nfloat velocity)
		{
			nfloat xOrigin = LeftContainerView.Frame.X;
			nfloat finalXOrigin = 0.0f;

			CGRect frame = LeftContainerView.Frame;
			frame.X = finalXOrigin;

			nfloat duration = SlideMenuOptions.AnimationDuration;

			if (velocity != 0)
			{
				duration = (nfloat) Math.Abs(xOrigin - finalXOrigin) / velocity;
				duration = (nfloat) Math.Max(0.1f, Math.Min(1.0f, duration));
			}

			AddShadowToView(LeftContainerView);

			UIView.Animate(duration, 0.0f, 0, 
			               () => {

							   LeftContainerView.Frame = frame;
							   OpacityView.Layer.Opacity = (float) SlideMenuOptions.ContentViewOpacity;

							   if (SlideMenuOptions.ContentViewDrag == true)
							   {
								   MainContainerView.Transform = CGAffineTransform.MakeTranslation(SlideMenuOptions.LeftViewWidth, 0);
							   }
							   else
							   {
								   MainContainerView.Transform = CGAffineTransform.MakeScale(SlideMenuOptions.ContentViewScale, SlideMenuOptions.ContentViewScale);
								}
							  
			}, 
			               () => {
							   DisableContentInteraction();
							   if (LeftViewController != null){
								   LeftViewController.EndAppearanceTransition();
						}
							   if (LeftDidOpen != null)
								{
								   LeftDidOpen();
								}
			});
		}

		public void OpenRightWithVelocity(nfloat velocity)
		{
			nfloat xOrigin = RightContainerView.Frame.X;
			nfloat finalXOrigin = View.Bounds.Width - RightContainerView.Frame.Size.Width;

			CGRect frame = RightContainerView.Frame;
			frame.X = finalXOrigin;

			nfloat duration = SlideMenuOptions.AnimationDuration;

			if (velocity != 0)
			{
				duration = (nfloat)Math.Abs(xOrigin - View.Bounds.Width) / velocity;
				duration = (nfloat)Math.Max(0.1f, Math.Min(1.0f, duration));
			}

			AddShadowToView(RightContainerView);

			UIView.Animate(duration, 0.0f, 0,
						   () =>
						   {

							   RightContainerView.Frame = frame;
							   OpacityView.Layer.Opacity = (float)SlideMenuOptions.ContentViewOpacity;

							   if (SlideMenuOptions.ContentViewDrag == true)
							   {
								   MainContainerView.Transform = CGAffineTransform.MakeTranslation(-SlideMenuOptions.RightViewWidth, 0);
							   }
							   else
							   {
								   MainContainerView.Transform = CGAffineTransform.MakeScale(SlideMenuOptions.ContentViewScale, SlideMenuOptions.ContentViewScale);
							   }
							   
						   },
						   () =>
						   {
							   DisableContentInteraction();
							   if (RightViewController != null)
							   {
								   RightViewController.EndAppearanceTransition();
							   }
							   if (RightDidOpen != null)
							   {
								   RightDidOpen();
							   }
						   });
		}


		public void CloseLeftWithVelocity(nfloat velocity)
		{ 
			nfloat xOrigin = LeftContainerView.Frame.X;
			nfloat finalXOrigin = LeftMinOrigin();

			CGRect frame = LeftContainerView.Frame;
			frame.X = finalXOrigin;

			nfloat duration = SlideMenuOptions.AnimationDuration;

			if (velocity != 0)
			{
				duration = (nfloat)Math.Abs(xOrigin - finalXOrigin) / velocity;
				duration = (nfloat)Math.Max(0.1f, Math.Min(1.0f, duration));
			}

			UIView.Animate(duration, 0.0f, 0,
						   () =>
						   {

							   LeftContainerView.Frame = frame;
							   OpacityView.Layer.Opacity = 0;

							   MainContainerView.Transform = CGAffineTransform.MakeScale(1, 1);
						   },
						   () =>
						   {
							   RemoveShadow(LeftContainerView);
							   EnableContentInteraction();

							   if (LeftViewController != null)
							   {
								   LeftViewController.EndAppearanceTransition();
							   }
							   if (LeftDidClose != null)
							   {
								   LeftDidClose();
							   }
						   });
		
		}

		public void CloseRightWithVelocity(nfloat velocity)
		{
			nfloat xOrigin = RightContainerView.Frame.X;
			nfloat finalXOrigin = View.Bounds.Width;

			CGRect frame = RightContainerView.Frame;
			frame.X = finalXOrigin;

			nfloat duration = SlideMenuOptions.AnimationDuration;

			if (velocity != 0)
			{
				duration = (nfloat)Math.Abs(xOrigin - View.Bounds.Width) / velocity;
				duration = (nfloat)Math.Max(0.1f, Math.Min(1.0f, duration));
			}

			UIView.Animate(duration, 0.0f, 0,
					 () =>
					 {

						 RightContainerView.Frame = frame;
						 OpacityView.Layer.Opacity = 0;

						 MainContainerView.Transform = CGAffineTransform.MakeScale(1, 1);
					 },
					 () =>
					 {
						 RemoveShadow(RightContainerView);
						 EnableContentInteraction();

						 if (RightViewController != null)
						 {
							 RightViewController.EndAppearanceTransition();
						 }
						 if (RightDidClose != null)
						 {
							 RightDidClose();
						 }
					 });

		}

		// Change Left/Right/Main controller -----------------------------------------------------------------

		public void ChangueMainViewcontroller(UIViewController mainViewcontroller, bool close)
		{
			RemoveViewController(MainViewController);
			MainViewController = mainViewcontroller;
			SetUpViewController(MainContainerView, MainViewController);

			if (close)
			{
				CloseLeft();
				CloseRight();
			}

		}


		public void ChangueLeftViewcontroller(UIViewController LeftViewcontroller, bool close)
		{
			RemoveViewController(LeftViewController);
			LeftViewController = LeftViewcontroller;
			SetUpViewController(LeftContainerView, LeftViewController);

			if (close)
			{
				CloseLeft();
			}

		}


		public void ChangeLeftViewWidth(nfloat width)
		{
			SlideMenuOptions.LeftViewWidth = width;

			CGRect LeftFrame = View.Bounds;
			LeftFrame.Width = width;
			LeftFrame.X = LeftMinOrigin();

			nfloat LeftOffset = 0;
			LeftFrame.Y = LeftFrame.Y + LeftOffset;
			LeftFrame.Height = LeftFrame.Height - LeftOffset;
			LeftContainerView.Frame = LeftFrame;
		}

		public void ChangeRightViewWidth(nfloat width)
		{
			SlideMenuOptions.RightBezelWidth = width;

			CGRect RightFrame = View.Bounds;
			RightFrame.Width = width;
			RightFrame.X = RightMinOrigin();

			nfloat RightOffset = 0;
			RightFrame.Y = RightFrame.Y + RightOffset;
			RightFrame.Height = RightFrame.Height - RightOffset;
			RightContainerView.Frame = RightFrame;
		}

		public void ChangueRightViewcontroller(UIViewController rightViewcontroller, bool close)
		{
			RemoveViewController(RightViewController);
			RightViewController = rightViewcontroller;
			SetUpViewController(RightContainerView, RightViewController);

			if (close)
			{
				CloseRight();
			}

		}

		PanInfo PanLeftResultInfoForVelocity(CGPoint velocity)
		{ 
			nfloat ThresholdVelocity  = 1000.0f;
			nfloat PointOfNoReturn = (nfloat) Math.Floor(LeftMinOrigin()) + SlideMenuOptions.PointOfNoReturnWidth;
			nfloat LeftOrigin = LeftContainerView.Frame.X;

			PanInfo panInfo = new PanInfo();
			panInfo.action = SlideAction.Close;
			panInfo.ShouldBounce = false;
			panInfo.Velocity = 0.0f;

			panInfo.action = LeftOrigin <= PointOfNoReturn ? SlideAction.Close : SlideAction.Open;

			if (velocity.X >= ThresholdVelocity) 
			{
				panInfo.action = SlideAction.Open;
				panInfo.Velocity = velocity.X;
			}
			else if (velocity.X <= (-1.0 * ThresholdVelocity) )
			{
				panInfo.action = SlideAction.Close;
				panInfo.Velocity = velocity.X;
			}

			return panInfo;
		}


		PanInfo PanRightResultInfoForVelocity(CGPoint velocity)
		{
			nfloat ThresholdVelocity = -1000.0f;
			nfloat PointOfNoReturn = (nfloat)Math.Floor(View.Bounds.Width) - SlideMenuOptions.PointOfNoReturnWidth;
			nfloat RightOrigin = RightContainerView.Frame.X;

			PanInfo panInfo = new PanInfo();
			panInfo.action = SlideAction.Close;
			panInfo.ShouldBounce = false;
			panInfo.Velocity = 0.0f;

			panInfo.action = RightOrigin >= PointOfNoReturn ? SlideAction.Close : SlideAction.Open;

			if (velocity.X <= ThresholdVelocity)
			{
				panInfo.action = SlideAction.Open;
				panInfo.Velocity = velocity.X;
			}
			else if (velocity.X >= (-1.0 * ThresholdVelocity))
			{
				panInfo.action = SlideAction.Close;
				panInfo.Velocity = velocity.X;
			}

			return panInfo;
		}

		CGRect ApplyLeftTranslation(CGPoint translation, CGRect toFrame)
		{ 
			nfloat newOrigin = toFrame.X;
			newOrigin += translation.X;

			nfloat minOrigin = LeftMinOrigin();
			nfloat maxOrigin = 0.0f;
			CGRect newFrame = toFrame;

			if (newOrigin < minOrigin) 
			{
				newOrigin = minOrigin;
			}
			else if (newOrigin > maxOrigin) 
			{
				newOrigin = maxOrigin;
			}

			newFrame.X = newOrigin;
			return newFrame;
		}

		CGRect ApplyRightTranslation(CGPoint translation, CGRect toFrame)
		{
			nfloat newOrigin = toFrame.X;
			newOrigin += translation.X;

			nfloat minOrigin = RightMinOrigin();
			nfloat maxOrigin = RightMinOrigin() - RightContainerView.Frame.Size.Width;
			CGRect newFrame = toFrame;

			if (newOrigin > minOrigin)
			{
				newOrigin = minOrigin;
			}
			else if (newOrigin < maxOrigin)
			{
				newOrigin = maxOrigin;
			}

			newFrame.X = newOrigin;
			return newFrame;
		}

		nfloat GetOpenedLeftRatio()
		{ 
			nfloat width = LeftContainerView.Frame.Size.Width;
			nfloat currentPosition = LeftContainerView.Frame.X - LeftMinOrigin();

			return currentPosition / width;
		
		}

		nfloat GetOpenedRightRatio()
		{ 
			nfloat width = RightContainerView.Frame.Size.Width;
			nfloat currentPosition = RightContainerView.Frame.X;

			return -(currentPosition - View.Bounds.Width) / width;
		}

		void ApplyLeftOpacity()
		{ 
			nfloat openedLeftRatio = GetOpenedLeftRatio();
			nfloat opacity = SlideMenuOptions.ContentViewOpacity * openedLeftRatio;
			OpacityView.Layer.Opacity = (float) opacity;
		}

		void ApplyRightOpacity()
		{ 
			nfloat openedRightRatio = GetOpenedRightRatio();
			nfloat opacity = SlideMenuOptions.ContentViewOpacity * openedRightRatio;
			OpacityView.Layer.Opacity = (float) opacity;
		}

		void ApplyLeftContentViewScale()
		{ 
			nfloat openedLeftRatio = GetOpenedLeftRatio();
			nfloat scale = 1.0f - ((1.0f - SlideMenuOptions.ContentViewScale) * openedLeftRatio);
			nfloat drag = SlideMenuOptions.LeftViewWidth + LeftContainerView.Frame.X;

			if (SlideMenuOptions.ContentViewDrag == true)
			{
				MainContainerView.Transform = CGAffineTransform.MakeTranslation(drag, 0);
			}
			else
			{
				MainContainerView.Transform = CGAffineTransform.MakeScale(scale, scale);
			}

		}

		void ApplyRightContentViewScale()
		{
			nfloat openedRightRatio = GetOpenedRightRatio();
			nfloat scale = 1.0f - ((1.0f - SlideMenuOptions.ContentViewScale) * openedRightRatio);
			nfloat drag = RightContainerView.Frame.X - MainContainerView.Frame.Size.Width;

			if (SlideMenuOptions.ContentViewDrag == true)
			{
				MainContainerView.Transform = CGAffineTransform.MakeTranslation(drag, 0);
			}
			else
			{
				MainContainerView.Transform = CGAffineTransform.MakeScale(scale, scale);
			}

		}

		void AddShadowToView(UIView targetContainerView)
		{ 
			targetContainerView.Layer.MasksToBounds = false;
			targetContainerView.Layer.ShadowOffset = SlideMenuOptions.ShadowOffset;
			targetContainerView.Layer.ShadowOpacity = (float) SlideMenuOptions.ShadowOpacity;
			targetContainerView.Layer.ShadowRadius = SlideMenuOptions.ShadowRadius;
			targetContainerView.Layer.ShadowPath = UIBezierPath.FromRect(targetContainerView.Bounds).CGPath;
		}

		void RemoveShadow(UIView targetContainerView)
		{
			targetContainerView.Layer.MasksToBounds = true;
			MainContainerView.Layer.Opacity = 1.0f;
		}

		void RemoveContentOpacity()
		{
			OpacityView.Layer.Opacity = 0;
		}

		void AddContentOpacity()
		{
			OpacityView.Layer.Opacity = (float)SlideMenuOptions.ContentViewOpacity;
		}

		void DisableContentInteraction()
		{
			MainContainerView.UserInteractionEnabled = false;
		}

		void EnableContentInteraction()
		{ 
			MainContainerView.UserInteractionEnabled = true;
		}

		void SetOpenWindowLevel()
		{
			if (SlideMenuOptions.HideStatusBar)
			{
				InvokeOnMainThread(() => {
					UIWindow window = UIApplication.SharedApplication.KeyWindow;
					if (window != null)
					{
						window.WindowLevel = UIWindowLevel.StatusBar + 1;
					}
				});
			}
		}

		void SetCloseWindowLevel()
		{
			if (SlideMenuOptions.HideStatusBar)
			{
				InvokeOnMainThread(() =>
				{
					UIWindow window = UIApplication.SharedApplication.KeyWindow;
					if (window != null)
					{
						window.WindowLevel = UIWindowLevel.Normal;
					}
				});
			}
		}

		void SetUpViewController(UIView targetView, UIViewController targetViewController)
		{
			if (targetViewController != null)
			{
				AddChildViewController(targetViewController);
				targetViewController.View.Frame = targetView.Bounds;
				targetView.AddSubview(targetViewController.View);
				targetViewController.DidMoveToParentViewController(this);
			}
		}

		void RemoveViewController(UIViewController viewController)
		{
			if (viewController != null)
			{
				viewController.View.Layer.RemoveAllAnimations();
				viewController.WillMoveToParentViewController(null);
				viewController.View.RemoveFromSuperview();
				viewController.RemoveFromParentViewController();
			}
		}

		public void CloseLeftNonAnimation()
		{ 
			SetCloseWindowLevel();
			nfloat finalXOrigin = LeftMinOrigin();
			CGRect frame = LeftContainerView.Frame;
			frame.X = finalXOrigin;
			LeftContainerView.Frame = frame;
			OpacityView.Layer.Opacity = 0.0f;
			MainContainerView.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
			RemoveShadow(LeftContainerView);
			EnableContentInteraction();
		}

		public void CloseRightNonAnimation()
		{ 
			SetCloseWindowLevel();
			nfloat finalXOrigin = View.Bounds.Width;
			CGRect frame = RightContainerView.Frame;
			frame.X = finalXOrigin;
			RightContainerView.Frame = frame;
			OpacityView.Layer.Opacity = 0.0f;
			MainContainerView.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
			RemoveShadow(RightContainerView);
			EnableContentInteraction();
		}

		// UIGestureRecognizerDelegate -----------------------------------------------------------------

		public bool ShoudReceiveTouch(UIGestureRecognizer gesture, UITouch touch)
		{
			CGPoint point = touch.LocationInView(View);

			if (gesture == LeftPanGesture)
			{
				return SlideLeftForGestureRecognizer(gesture, point);
			}
			else if (gesture == RightPanGesture)
			{
				return SlideRightForGestureRecognizer(gesture, point);
			}
			else if (gesture == LeftTapGesture)
			{
				return IsLeftOpen() && !IsPointContainedWithinLeftRect(point);
			}
			else if (gesture == RightTapGesture)
			{
				return IsRightOpen() && !IsPointContainedWithinRightRect(point);
			}

			return true;
		}

		public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gesture, UIGestureRecognizer othrtGesture)
		{
			return SlideMenuOptions.SimultaneousGestureRecognizers;
		}

		bool SlideLeftForGestureRecognizer(UIGestureRecognizer gesture, CGPoint point)
		{
			return IsLeftOpen() || SlideMenuOptions.PanFromBezel && IsLeftPointContainedWithinBezelRect(point);
		}

		bool SlideRightForGestureRecognizer(UIGestureRecognizer gesture, CGPoint point)
		{
			return IsRightOpen() || SlideMenuOptions.RightPanFromBezel && IsRightPointContainedWithinBezelRect(point);
		}

		bool IsLeftPointContainedWithinBezelRect(CGPoint point)
		{
			nfloat bezelWidth = SlideMenuOptions.LeftBezelWidth;
			CGRect leftBezelRect = new CGRect(0,0,0,0);
			CGRect tempRect = new CGRect(0,0,0,0);
			View.Bounds.Divide(bezelWidth, CGRectEdge.MinXEdge, out leftBezelRect, out tempRect);

			return leftBezelRect.Contains(point);

		}

		bool IsRightPointContainedWithinBezelRect(CGPoint point)
		{
			nfloat rightBezelWidth = SlideMenuOptions.RightBezelWidth;
			CGRect rightBezelRect = new CGRect(0,0,0,0);
			CGRect tempRect = new CGRect(0,0,0,0);

			nfloat bezelWidth = View.Bounds.Width - rightBezelWidth;
			View.Bounds.Divide(bezelWidth, CGRectEdge.MinXEdge, out rightBezelRect, out tempRect);
			return rightBezelRect.Contains(point);

		}

		bool IsPointContainedWithinLeftRect(CGPoint point)
		{
			return LeftContainerView.Frame.Contains(point);
		}

		bool IsPointContainedWithinRightRect(CGPoint point)
		{
			return RightContainerView.Frame.Contains(point);
		}



	}
}

