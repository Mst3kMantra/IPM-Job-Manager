   M S B A M L   `   `   `  ���� J  FIPM Job Manager, Version=1.0.4.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:IPM_Job_Manager_netIPM_Job_Manager_net  %    IPM_Job_Manager_net.AdminWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:IPM_Job_Manager_net     -     � Title$  IPM Job Manager��$	��800��$
��1350�� � WindowStartupLocation$ CenterOwner=�.��� $��10q� � RowDefinitions �� $
��Auto ��� $��* � � ColumnDefinitions �� $��0 ��� $��0 ��� $
��200* ��� $
��200* ��� $	��55* �.+���    
LabelTitle#��[  $��4��$��2��$
��10pxq�$��0,0,0,0q�
�������$��2��$	��Top=�.��	Employees�� #��[  $��2��$
��10pxq�
�������$��0,0,5,0q�.��Jobs�� #��[  $��3��$
��10pxq�
�������$��0,0,5,0q�.��Assigned Jobs�� -    ��Name$ lstUsers��$��1��$��	0,0,0,356q�$��4������ *
UserList+ �� Mode$ OneWay=����� .R���  �� Orientation$ 
Horizontal=�.+��� ����  �� Path$ Username��� -    ��Name$ lstJobs��$��1��$��2��$��0,0,5,0q����� *	JobList+$ OneWay=�	 �� View	 �� 
 �� AllowsColumnReorder
 .  �� ColumnHeaderToolTip$ Assigned Job Information��.P��� $��Job No.�� �� Width$ 80�� �� CellTemplate �� .R���  �� TextAlignment$ Center=����� $ JobInfo[JobNo]��� $��Customer��$	 100�� �� .R��� $ Center=�$
��Wrap=����� $ JobInfo[CustDesc]��� $��Part No.��$	 100�� �� .R��� $ Center=�$
��Wrap=����� $ JobInfo[PartNo]��� $��Part Description��$	 225�� �� .R��� $
��Wrap=����� $ JobInfo[PartDesc]��� $��Priority��$ 50�� �� .R��� $
��Wrap=����� $ JobInfo[Priority]��� $��3��$��1��$��0,0,5,0q�.+��� -   $ lstAssignedJobs��$��Stretch=�$	��400��$��1��$��3������ *CurEmployeeJobs+$ OneWay=�	 �� 
 . $ Assigned Job Information��.P��� $��Job No.��$ 80�� �� .R��� $ Center=����� $ JobInfo[JobNo]��� $��Customer��$	 100�� �� .R��� $ Center=�$
��Wrap=����� $ JobInfo[CustDesc]��� $��Part No.��$	 100�� �� .R��� $ Center=�$
��Wrap=����� $ JobInfo[PartNo]��� $��Part Description��$	 225�� �� .R��� $
��Wrap=����� $ JobInfo[PartDesc]��� $��Priority��$ 50�� �� .R��� $
��Wrap=����� $ JobInfo[Priority]�� 
������� �� $
��auto ��� $	��125 � �� $
��150* ��� $
��100* �.+��� $��Center=�  JobTitle#��[ .��
Operations�� $��1��$��Center=�#��[ .��
Assigned�� -   $ lstOperations��$��0��$��1������ *OperationList+���� .R��� ���� �� -   $ lstAssigned��$��1��$��1������ *AssignedEmployeeList+���� .R��� ���� �� 
�������#��[ .��Notes:�� -    ��Name$ txtNotes��$	��125��$
��Wrap=��� 
�������$��1��$��4��$��Bottom=�.+��� -    ��Name$ 
btnRefresh��$��0,5,0,0q�.��	Refresh�� -	   $ 	btnAssign��$��0,5,0,0q�.��
Assign Job�� -
   $ 	btnRemove��$��0,5,0,0q�.��
Remove Job�� -   $ 
btnAddPrio��$��0,5,0,0q�.��Increase Priority�� -   $ btnLowerPrio��$��0,5,0,0q�.��Reduce Priority�� -   $ btnView��$��0,5,0,0q�.��View Job Details�� -   $ btnEditOperations��$��0,5,0,0q�.��Edit Operations�� -   $ btnEditNotes��$��0,5,0,0q�.��
Edit Notes�� -   $ 
btnAddUser��$��0,5,0,0q�.��Add New User�� -   $ 	btnLogout��$��0,5,0,0q�$��0,0,0,0q�.��Logout