/*Clone object for use later and map out affected polys for review*/
actionMan.executeAction 0 "281"  -- Tools: Hide Unselected
maxOps.cloneNodes $ cloneType:#copy newNodes:&nnl
select nnl
subobjectLevel = 2
$.edgeChamfer = 1.5
$.EditablePoly.buttonOp #Chamfer
$.EditablePoly.GrowSelection ()
macros.run "Editable Polygon Object" "EPoly_Convert_Sel_To_Face"
$.EditablePoly.ShrinkSelection ()
