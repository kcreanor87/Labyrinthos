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
--Set UV's
subobjectLevel = 0
modPanel.addModToSelection (Uvwmap ()) ui:on
$.modifiers[#UVW_Map].maptype = 4
$.modifiers[#UVW_Map].length = 200
$.modifiers[#UVW_Map].width = 200
$.modifiers[#UVW_Map].height = 200
--Select duplicated mesh to create extruded walls*/
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
modPanel.addModToSelection (Uvwmap ()) ui:on
	$.modifiers[#UVW_Map].maptype = 5
actionMan.executeAction 0 "40021"  -- Selection: Select All
