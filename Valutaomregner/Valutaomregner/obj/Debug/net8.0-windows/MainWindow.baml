   M S B A M L   `   `   `  ����I  EValutaomregner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ValutaomregnerValutaomregner  5   	       Valutaomregner.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:Valutaomregner     5   	     � Title$  Valuta Omregner��5   	   $	��200��6!   $	��525��6.    � 	Resources 5	       �� 
TargetType �� Property �� Value��%V   '��       �� 5
   
   " ��6   .�5      ��  � ��$ Center=�69   5      5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Valuta Omregner��6   $��20(�6+   $
��Bold"�69   $��Center=�6K   $��0,0,0,20q�6h   �� 5   
    �� Orientation$ 
Horizontal=�6   $��1��6.   $��0,0,0,10q�6;   $��Center=�6M   .+�5      �� $
��Fra:��6   $��60��6#   �� 5      -   5         ��Name$ cmbFromCurrency��5      $	��150��60   6<   �� 5      -   5         ��Name$ btnSwapCurrencies��5      $	��⇄��60   $��30��6<   $��	10,0,10,0q�6G   6Z   �� 5      $
��Til:��6   $��60��6#   �� 5       -   5        $ cmbToCurrency��5       $	��150��6.   	 �� DisplayMemberPath$	 DisplayName��6g   5!      �� 5$   
   $ 
Horizontal=�6   $��2��6.   $��0,0,0,10q�6;   .+�5%      �� $��Beløb:��6   $��60��6%   �� 5&      -   5        
 ��Name$
 	txtAmount��5&      $	��150��6)   65   �� 5'      $�� ��6   $��50��6   �� 5(      $��	Resultat:��6   $��60��6(   �� 5)      -   5        $
 txtConvertedAmount��5)      $	��150��62    �� 
IsReadOnly$
 True��6>   5*      5+      5,      