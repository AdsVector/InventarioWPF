   M S B A M L   `   `   `  ����H  DInventarioWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:InventarioWPF.VentanasInventarioWPF.Ventanas  5   	   ;"clr-namespace:InventarioWPF.PageCSInventarioWPF.PageCS  5   	   -clr-namespace:InventarioWPFInventarioWPF  5   	   )    #InventarioWPF.Ventanas.Reg_Sesiones   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:InventarioWPF.Ventanas     5   	   5pagecs"clr-namespace:InventarioWPF.PageCS     5   	   ,mainclr-namespace:InventarioWPF     5   	   X TXceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5	   	   -   5          � Title$  Regristro De Sesiones��5   	   $	��450��6'   $	��800��64    � 
ResizeMode$ NoResize=�5   	    � ShowInTaskbar .   � 	Resources 5      )   #InventarioWPF.BoolToStringConverter 
  CYesNo�   �InventarioWPF.PageCS.BoolToValueConverter`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   
FalseValue   	TrueValue.  (InventarioWPF.PageCS.IntToColorConverter  
IntToColor/  )InventarioWPF.PageCS.StringToUriConverter  StringToUri   modeGetPicture��%~   &	        &	 *     &	 =       5   
   $ NO��64   $ SI��6D     5   
   6%     5   
   $ 	Empleados=�6:   5      .��5      � .+�5   
   � -   5         �Name$ AllElements��5       � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   	 � RowDefinitions	 5      �� 5      $��35 �6    �� 5      $��4* �6    �� 5      $��40 �6    .+�5      �� -   5        
 ��Name$
 	txtSearch��5      
�������$��10, 10, 20, 0q�6<   $��14(�6S   $��Center=�6a   6�   &   System.Windows.Controls.DataGrid  5!      -   5          Name$ dgReg��5!         AutoGenerateColumns .  $��1��6B   $��3��6O   $��10q�6c   ���	#FFF1F1F1   RowBackground �	#FFFFFFFF   AlternatingRowBackground �	#FFF1F1F1   GridLinesVisibility$ 
Horizontal=�5"   V      SelectionMode$ Single=�6w      HorizontalGridLinesBrush �#CCCCCC  DGHeaderStyle#��[ 5#   ;      HeadersVisibility$ Column=�6b      
IsReadOnly .    Columns 5$      0  *System.Windows.Controls.DataGridTextColumn  5%      ,  &System.Windows.Controls.DataGridColumn   Header$
 Nro.��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6N   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 67   �� *Id+   	CellStyle 5&      �� 5'       �� 
TargetType*
  $System.Windows.Controls.DataGridCell" 
 6$     DGCellStyle �� BasedOn# [ 6G   .�5(   "   ��  �� Property �@��TextAlignment �� Value$ Center=�6H   5)      5+        5,      $ Empleado��6)   $ 3* 6X    6;   �� *
Empleado+6X     5-      $ Entrada��6)   $
 Auto 6T    6:   �� *Entry+ 5.      �� 5/      " 
 6$   # [ 6G   .�50   "   ��  �@��TextAlignment$ Center=�6H   51      53        54      $ Salida��6)   $ 2* 6R    69   �� *Exit+ 55      �� 56      " 
 6$   # [ 6G   .�57   "   ��  �@��TextAlignment$ Center=�6H   58      5:      5<      �� 5>      $��3��6   ���#2ecc71$��2��67   ._�5?      �� -   5         ��Name$ 
txtNumbers��5?      
�������$��20(�6C   $��Stretch=�6Q   $��10, 0, 0, 0q�6o   $��Center=�6�   $%��No se han encontrado registros.��6�   5@      �� 5B       �� Orientation$ 
Horizontal=�6   $��3��62   $��1��6?   .+�5C      �� -   5          ButtonStyleGreen#��[ 5C      .��5D      �� $6��0pack://siteoforigin:,,,/Resources/first_48px.png��6   5E      �� 5F      -   5        #��[ 5F      $��1��6C   .��5G      �� $9��3pack://siteoforigin:,,,/Resources/previous_48px.png��6   5H      �� 5I      -   5        #��[ 5I      $��2��6C   .��5J      �� $5��/pack://siteoforigin:,,,/Resources/next_48px.png��6   5K      �� 5L      -	   5        #��[ 5L      $��3��6C   .��5M      �� $5��/pack://siteoforigin:,,,/Resources/last_48px.png��6   5N      5O      5P      %  Xceed.Wpf.Toolkit.BusyIndicator  5R   
   -
   5          Name$ bussy��5R          IsBusy  . !   BusyContent$! Cargando Información...��6:   5S      5T      