actionMan.executeAction 0 "40044"  -- Selection: Select Invert
actionMan.executeAction 0 "40020"  -- Edit: Delete Objects
$.EditablePoly.delete #Face
modPanel.addModToSelection (Shell ()) ui:on
$.modifiers[#Shell].innerAmount = 05
$.modifiers[#Shell].selectEdgeFaces = on
subobjectLevel = 0
macros.run "Modifier Stack" "Convert_to_Poly"
subobjectLevel = 4
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 3
actionMan.executeAction 0 "40044"
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 1
$.insetAmount = 1
$.EditablePoly.buttonOp #Inset
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 2
actionMan.executeAction 0 "40044"  -- Selection: Select Invert
$.EditablePoly.autosmooth ()
subobjectLevel = 4 
modPanel.addModToSelection (Uvwmap ()) ui:on
$.modifiers[#UVW_Map].maptype = 5
subobjectLevel = 0
	macros.run "Modifier Stack" "Convert_to_Poly"