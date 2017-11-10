subobjectLevel = 2
$.edgeChamferSegments = 1
$.edgeChamfer = 1.85
$.edgeChamferType = 0
$.EditablePoly.buttonOp #Chamfer
$.EditablePoly.GrowSelection ()
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 1
$.EditablePoly.ShrinkSelection ()
actionMan.executeAction 0 "40044"
$.EditablePoly.delete #Face
actionMan.executeAction 0 "40021"
$.bevelType = 1
$.bevelHeight = 1.57
$.bevelOutline = -0.52
$.EditablePoly.buttonOp #Bevel
$.bevelHeight = 0.3
$.bevelOutline = -0.4
$.EditablePoly.buttonOp #Bevel
$.insetAmount = 0.25
$.EditablePoly.buttonOp #Inset
$.bevelOutline = -0.2
$.bevelHeight = 0.35
$.EditablePoly.buttonOp #Bevel
$.EditablePoly.GrowSelection ()
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 2
$.EditablePoly.ShrinkSelection ()
$.insetAmount = 0.12
$.EditablePoly.buttonOp #Inset
$.EditablePoly.detachToElement #Face keepOriginal:on
$.bevelHeight = 9
$.bevelOutline = -0.1
$.EditablePoly.buttonOp #Bevel
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Vertex"
$.weldThreshold = 0.84
$.EditablePoly.weldFlaggedVertices ()
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
polyOp.setFaceMatID $ (polyOp.GetFaceSelection $) 3
actionMan.executeAction 0 "40021"
$.autoSmoothThreshold = 35
$.EditablePoly.autosmooth ()
subobjectLevel = 0
modPanel.addModToSelection (Uvwmap ()) ui:on
$.modifiers[#UVW_Map].maptype = 5

