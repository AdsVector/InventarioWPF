   M S B A M L   `   `   `  ����H  DInventarioWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:InventarioWPF.VentanasInventarioWPF.Ventanas  5   	   -clr-namespace:InventarioWPFInventarioWPF  5   	   #    InventarioWPF.Ventanas.Cargos   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:InventarioWPF.Ventanas     5   	   ,mainclr-namespace:InventarioWPF     5   	   X TXceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   -   5          � Title$  	Empleados��5
   	   $	��550��6   $
��1000��6(   $	��550��65   $
��1000��5   	    � ShowInTaskbar .   � 	Resources 5      )   #InventarioWPF.BoolToStringConverter 
  CYesNo�   �InventarioWPF.PageCS.BoolToValueConverter`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   
FalseValue   	TrueValue��%4   &	          5   
   $ NO��64   $ SI��6D   5      .��5      � .+�5   
   � -   5         �Name$ AllElements��5       � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $
��Auto �6#   �� 5      $
��Auto �6#    � RowDefinitions 5      �� 5      $��35 �6    �� 5      $��4* �6    �� 5      $��40 �6    .+�5      �� -   5         ��Name$ 	txtSearch��5      
�������$��10, 10, 20, 0q�6<   $��14(�6S   $��Center=�6a   6�   &   System.Windows.Controls.DataGrid  5      -   5        	  Name$	 dgEmp��5      
   AutoGenerateColumns
 .  $��1��6B   $��3��6O   $��10q�6c     
otherPoint � ContextMenu# �  6o   ���	#FFF1F1F1   RowBackground �	#FFFFFFFF   AlternatingRowBackground �	#FFF1F1F1   GridLinesVisibility$ 
Horizontal=�5    m      SelectionMode$ Single=�6�      HorizontalGridLinesBrush �#CCCCCC  DGHeaderStyle#��[ 5!   ;      HeadersVisibility$ Column=�6b      
IsReadOnly .  5"        rowPoint��%�  &	       &	 �     �� 5#      .@�5$      m� -   5        $��Agregar Nuevo��5$   #   6:   �� 5%      m� 5&      -   5        $��
Actualizar��5&   #   67   5'      �� 5)      .@�5*      m� -   5        $��Agregar Nuevo��5*   #   6:   m� 5+      -   5        $��Editar Registro��5+   #   6<   �� 5,      m� 5-      -	   5        $��
Actualizar��5-   #   67   5.      5/         RowStyle 51      �� 52       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�53      ��  �� Property �@�ContextMenu �� Value# �  68   54         Columns 57      0  *System.Windows.Controls.DataGridTextColumn  58      ,  &System.Windows.Controls.DataGridColumn   Header$
 Nro.��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6R   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *Id_Dep+   	CellStyle 59      �� 5:      *	  $System.Windows.Controls.DataGridCell" 	 6$     DGCellStyle �� BasedOn# [ 6G   .�5;   "   ��  �@��TextAlignment$ Center=�6H   5<      5>        5?      $ Nombre��6)   $ * 6T    69   �� *Nombre+6T     5@      $ 
Inventario��6)   $
 Auto 6    6=   �� *
Inventario+ �� 	Converter# [   5A      �� 5B      " 	 6$   # [ 6G   .�5C   "   ��  �@��TextAlignment$ Center=�6H   5D      5F        5G      $ Proveedores��6)   $
 Auto 6�    6>   �� *Proveedores+# [   5H      �� 5I      " 	 6$   # [ 6G   .�5J   "   ��  �@��TextAlignment$ Center=�6H   5K      5M        5N      $ Ventas��6)   $
 Auto 6w    69   �� *Ventas+# [   5O      �� 5P      " 	 6$   # [ 6G   .�5Q   "   ��  �@��TextAlignment$ Center=�6H   5R      5T        5U      $ 	Empleados��6)   $
 Auto 6}    6<   �� *	Empleados+# [   5V      �� 5W      " 	 6$   # [ 6G   .�5X   "   ��  �@��TextAlignment$ Center=�6H   5Y      5[        5\      $ Departamentos��6)   $
 Auto 6�    6@   �� *Departamentos+# [   5]      �� 5^      " 	 6$   # [ 6G   .�5_   "   ��  �@��TextAlignment$ Center=�6H   5`      5b        5c      $ Reg. Sesiones��6)   $
 Auto 6�    6@   �� *RegSesiones+# [   5d      �� 5e      " 	 6$   # [ 6G   .�5f   "   ��  �@��TextAlignment$ Center=�6H   5g      5i      5k      �� 5m      $��3��6   ���#2ecc71$��2��67   ._�5n      �� -
   5         ��Name$ 
txtNumbers��5n      
�������$��20(�6C   $��Stretch=�6Q   $��10, 0, 0, 0q�6o   $��Center=�6�   $%��No se han encontrado registros.��6�   5o      �� 5q       �� Orientation$ 
Horizontal=�6   $��3��62   $��3��6?   .+�5r      �� -   5          ButtonStyleGreen#��[ 5r      .��5s      �� $6��0pack://siteoforigin:,,,/Resources/first_48px.png��6   5t      �� 5u      -   5        #��[ 5u      $��1��6C   .��5v      �� $9��3pack://siteoforigin:,,,/Resources/previous_48px.png��6   5w      �� 5x      -   5        #��[ 5x      $��2��6C   .��5y      �� $5��/pack://siteoforigin:,,,/Resources/next_48px.png��6   5z      �� 5{      -   5        #��[ 5{      $��3��6C   .��5|      �� $5��/pack://siteoforigin:,,,/Resources/last_48px.png��6   5}      5~      5      �� 5�   
   -   5          ��Name$  	backColor��5�      
���   �! J� Opacity$! 0Q�67   $y�	Collapsed=�6C   �� 5�   
   -   5        $  AddUI��5�      " �� CornerRadius$" 20��6    $	��500��62   $��Center=�6>   # J� RenderTransformOrigin$# 0.5,0.5&�6Y   
�������$ �� Padding$$ 5q�6�   z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      % T� Y% 6)   �� *ActualHeight+& �� Mode$& OneWay=�' �� RelativeSource' �� *FindAncestor+( �� AncestorType#
   InventarioWPF.Ventanas.Cargos"( 
 5�      ._�5�      �  5�      �� 5�      $
��Auto �6'   �� 5�      $��* �6'    5�      �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   .+�5�      �� $��	Nombre: *��6   $��16(�6-   $��10, 5, 5, 5q�6;   $��0��6P   �� 5�      -   5        $ txtNom��5�      $��14(�6*   $��5q�68   $��2q�6C   $��0��6O   $��1��6\   �� 5�      $ 
Horizontal=�6   $��1��66   $��2��6C   .+�5�      �� -   5        ) ��Name$) cbInv��5�      $��
Inventario��6*   $��1��6?   $��5q�6L   �� 5�      -   5        $) cbPro��5�      $��Proveedores��6*   $��1��6@   $��1��6M   $��5q�6]   �� 5�      -   5        $) cbVen��5�      $��Ventas��6*   $��1��6;   $��2��6H   $��5q�6X   5�      �� 5�      $ 
Horizontal=�6   $��2��66   $��2��6C   .+�5�      �� -   5        $) cbEmp��5�      $��	Empleados��6.   $��2��6B   $��5q�6O   �� 5�      -   5        $) cbDep��5�      $��Departamentos��6.   $��2��6F   $��1��6S   $��5q�6c   �� 5�      -   5        $) cbReg��5�      $��Registro Sesiones��6.   $��2��6J   $��2��6W   $��5q�6g   5�      �� 5�      $ 
Horizontal=�6   $��2��66   $��7��6F   $��2��6S   $��30��6d   $��5q�6p   $��Right=�6{   .+�5�      �� -   5        * ��Name$* btnSave��5�      $��Guardar��6.   $��20(�6@   $	��100��6N   
�������#��[ 6m   6�   �� 5�      -   5        $* btnClose��5�      $��X��6/   $��20(�6;   $��50��6I   
�������  ButtonStyleRed#��[ 6g   6�   5�      5�      + � Triggers+ 5�      :� 5�      , :� RoutedEvent$, Border.Unloaded��6   .[�5�      �� -   5        - ��Name$- CloseBeginStoryBoard��5�   &   .��5�      �� -   5        . ��Name$. CloseStoryBoard��5�   %   .��5�      U� / �� 
TargetName$/ AddUI��6<   0 �� TargetProperty$U0 O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�5�   <   .\�5�   "   9  3System.Windows.Media.Animation.EasingDoubleKeyFrame  1 O� KeyTime$1 0��67   2 O� Value$2 0Q�6C     5�   "   $1 0:0:0.4��67   2 6I   �� *	MaxHeight+$& OneWay=�' �� *FindAncestor+"( 
 5�      5�      5�      5�      :� 5�      $, Border.Unloaded��6   .[�5�      �� -   5        $- ShowBeginStoryBoard��5�   &   .��5�      �� -   5        $. ShowStoryBoard��5�   %   .��5�      U� $/ AddUI��6<   $U0 O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�5�   <   .\�5�   "     $1 0��67   2 6C   �� *ActualHeight+$& OneWay=�' �� *FindAncestor+"( 
   5�   "   $1 0:0:0.8��67   $2 0Q�6I   5�      5�      5�      5�      5�      %  Xceed.Wpf.Toolkit.BusyIndicator  5�   
   -   5        3  Name$3 bussy��5�      4   IsBusy4 . 5   BusyContent$5 Cargando Información...��6:   5�      5�      