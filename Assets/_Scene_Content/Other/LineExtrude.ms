$.edgeExtrudeHeight = 10
$.edgeExtrudeWidth = 1
$.EditablePoly.buttonOp #Extrude
$.EditablePoly.GrowSelection ()
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
actionMan.executeAction 0 "40044"
actionMan.executeAction 0 "40020"
$.EditablePoly.delete #Face
subobjectLevel = 3
actionMan.executeAction 0 "40021"
$.edgeExtrudeHeight = 1
$.edgeExtrudeWidth = 1
$.EditablePoly.buttonOp #Extrude
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
$.autoSmoothThreshold = 25
$.EditablePoly.autosmooth ()
actionMan.executeAction 0 "40044"  -- Selection: Select Invert
$.EditablePoly.autosmooth ()

