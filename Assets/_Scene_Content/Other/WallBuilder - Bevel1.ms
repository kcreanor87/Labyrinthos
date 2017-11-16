subobjectLevel = 4
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 1
actionMan.executeAction 0 "40044"
$.EditablePoly.delete #Face
actionMan.executeAction 0 "40021"  -- Selection: Select All
$.extrusionType = 1
$.faceExtrudeHeight = 0.25f
$.EditablePoly.buttonOp #Extrude
$.insetAmount = 0.25
$.EditablePoly.buttonOp #Inset
$.faceExtrudeHeight = 1.5
$.EditablePoly.buttonOp #Extrude
$.EditablePoly.GrowSelection ()
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 2
$.EditablePoly.ShrinkSelection ()
$.bevelType = 1
$.bevelHeight = 0
$.bevelOutline = -0.18
$.bevelHeight = 0.15
$.EditablePoly.buttonOp #Bevel
$.EditablePoly.buttonOp #Inset

