modPanel.addModToSelection (Shell ()) ui:on
$.modifiers[#Shell].innerAmount = 20
$.modifiers[#Shell].selectEdgeFaces = on
subobjectLevel = 0
macros.run "Modifier Stack" "Convert_to_Poly"
subobjectLevel = 4
actionMan.executeAction 0 "40044"
$.insetAmount = 1
$.EditablePoly.buttonOp #Inset
actionMan.executeAction 0 "40044"  -- Selection: Select Invert
$.EditablePoly.autosmooth ()
subobjectLevel = 4 