/*
Copyright 2008-2010 Gephi
Authors : Eduardo Ramos <eduramiba@gmail.com>
Website : http://www.gephi.org

This file is part of Gephi.

DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS HEADER.

Copyright 2011 Gephi Consortium. All rights reserved.

The contents of this file are subject to the terms of either the GNU
General Public License Version 3 only ("GPL") or the Common
Development and Distribution License("CDDL") (collectively, the
"License"). You may not use this file except in compliance with the
License. You can obtain a copy of the License at
http://gephi.org/about/legal/license-notice/
or /cddl-1.0.txt and /gpl-3.0.txt. See the License for the
specific language governing permissions and limitations under the
License.  When distributing the software, include this License Header
Notice in each file and include the License files at
/cddl-1.0.txt and /gpl-3.0.txt. If applicable, add the following below the
License Header, with the fields enclosed by brackets [] replaced by
your own identifying information:
"Portions Copyrighted [year] [name of copyright owner]"

If you wish your version of this file to be governed by only the CDDL
or only the GPL Version 3, indicate your decision by adding
"[Contributor] elects to include this software in this distribution
under the [CDDL or GPL Version 3] license." If you do not indicate a
single choice of license, a recipient has the option to distribute
your version of this file under either the CDDL, the GPL Version 3 or
to extend the choice of license to its licensees as provided above.
However, if you add GPL Version 3 code and therefore, elected the GPL
Version 3 license, then the option applies only if the new code is
made subject to such option by the copyright holder.

Contributor(s):

Portions Copyrighted 2011 Gephi Consortium.
 */
package org.gephi.plugins.example.datalab.merge.columns;

import javax.swing.JPanel;
import javax.swing.event.DocumentEvent;
import javax.swing.event.DocumentListener;
import org.gephi.data.attributes.api.AttributeTable;
import org.gephi.datalab.spi.DialogControls;
import org.gephi.datalab.spi.Manipulator;
import org.gephi.datalab.spi.ManipulatorUI;
import org.gephi.ui.utils.ColumnTitleValidator;
import org.netbeans.validation.api.ui.ValidationGroup;
import org.netbeans.validation.api.ui.ValidationPanel;

/**
 * UI for EqualValuesMergeStrategy.
 * Demonstrates Netbeans Validation API
 * @author Eduardo Ramos <eduramiba@gmail.com>
 */
public class EqualValuesMergeStrategyUI extends javax.swing.JPanel implements ManipulatorUI {

    private EqualValuesMergeStrategy manipulator;
    private DialogControls dialogControls;
    private AttributeTable table;
    private String displayName;

    /** Creates new form GeneralColumnTitleChooserUI */
    public EqualValuesMergeStrategyUI() {
        initComponents();
        titleTextField.getDocument().addDocumentListener(new DocumentListener() {

            @Override
            public void insertUpdate(DocumentEvent e) {
                refreshOkButton();
            }

            @Override
            public void removeUpdate(DocumentEvent e) {
                refreshOkButton();
            }

            @Override
            public void changedUpdate(DocumentEvent e) {
                refreshOkButton();
            }
        });
    }

    private void refreshOkButton() {
        //Validate new column title for the ok button:
        String text = titleTextField.getText();
        dialogControls.setOkButtonEnabled(text != null && !text.isEmpty() && table != null && !table.hasColumn(text));//Title not empty and not repeated.
    }

    @Override
    public void setup(Manipulator m, DialogControls dialogControls) {
        this.manipulator = (EqualValuesMergeStrategy) m;
        this.table = manipulator.getTable();
        this.dialogControls = dialogControls;
        this.displayName = m.getName();
        titleTextField.setText(manipulator.getColumnTitle());
        refreshOkButton();
    }

    @Override
    public void unSetup() {
        manipulator.setColumnTitle(titleTextField.getText());//Communicate new column title (validated) to the manipulator before it is executed
    }

    @Override
    public String getDisplayName() {
        return displayName;//Use manipulator name as the dialog title
    }

    @Override
    public JPanel getSettingsPanel() {
        //Create a validation panel:
        ValidationPanel validationPanel = new ValidationPanel();
        validationPanel.setInnerComponent(this);

        ValidationGroup group = validationPanel.getValidationGroup();

        group.add(titleTextField, new ColumnTitleValidator(table));//ColumnTitleValidator is in UI utils module

        return validationPanel;
    }

    @Override
    public boolean isModal() {
        return true;
    }

    /** This method is called from within the constructor to
     * initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is
     * always regenerated by the Form Editor.
     */
    @SuppressWarnings("unchecked")
    // <editor-fold defaultstate="collapsed" desc="Generated Code">//GEN-BEGIN:initComponents
    private void initComponents() {

        titleLabel = new javax.swing.JLabel();
        titleTextField = new javax.swing.JTextField();

        titleLabel.setText(org.openide.util.NbBundle.getMessage(EqualValuesMergeStrategyUI.class, "EqualValuesMergeStrategyUI.titleLabel.text")); // NOI18N

        titleTextField.setText(org.openide.util.NbBundle.getMessage(EqualValuesMergeStrategyUI.class, "EqualValuesMergeStrategyUI.titleTextField.text")); // NOI18N

        javax.swing.GroupLayout layout = new javax.swing.GroupLayout(this);
        this.setLayout(layout);
        layout.setHorizontalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addComponent(titleLabel)
                .addPreferredGap(javax.swing.LayoutStyle.ComponentPlacement.RELATED)
                .addComponent(titleTextField, javax.swing.GroupLayout.DEFAULT_SIZE, 150, Short.MAX_VALUE)
                .addContainerGap())
        );
        layout.setVerticalGroup(
            layout.createParallelGroup(javax.swing.GroupLayout.Alignment.LEADING)
            .addGroup(layout.createSequentialGroup()
                .addContainerGap()
                .addGroup(layout.createParallelGroup(javax.swing.GroupLayout.Alignment.BASELINE)
                    .addComponent(titleLabel)
                    .addComponent(titleTextField, javax.swing.GroupLayout.PREFERRED_SIZE, javax.swing.GroupLayout.DEFAULT_SIZE, javax.swing.GroupLayout.PREFERRED_SIZE))
                .addContainerGap(javax.swing.GroupLayout.DEFAULT_SIZE, Short.MAX_VALUE))
        );
    }// </editor-fold>//GEN-END:initComponents
    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JLabel titleLabel;
    private javax.swing.JTextField titleTextField;
    // End of variables declaration//GEN-END:variables
}
