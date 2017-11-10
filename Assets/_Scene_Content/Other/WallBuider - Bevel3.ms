$.faceExtrudeHeight = 6.5
$.EditablePoly.buttonOp #Extrude
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Edge"
$.edgeChamferSegments = 1
$.edgeChamferType = 0
$.edgeChamfer = 0.16
$.edgeChamferSmoothThreshold = 30
$.EditablePoly.buttonOp #Chamfer
subobjectLevel = 4
actionMan.executeAction 0 "40021"
$.EditablePoly.autosmooth ()
subobjectLevel = 0
actionMan.executeAction 0 "40044"  -- Selection: Select Invert
subobjectLevel = 2
$.edgeExtrudeWidth = 0.25
$.edgeExtrudeHeight = 9.5
$.EditablePoly.buttonOp #Extrude
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 3
actionMan.executeAction 0 "40044"
$.EditablePoly.delete #Face
subobjectLevel = 0
actionMan.executeAction 0 "40021"  -- Selection: Select All
modPanel.addModToSelection (Uvwmap ()) ui:on
$.EditablePoly.modifiers[#UVW_Map].maptype = 5
