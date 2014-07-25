/*
Copyright 2008-2011 Gephi
Authors : Mathieu Bastian <mathieu.bastian@gephi.org>
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
package org.gephi.plugins.example.filter;

import org.gephi.filters.spi.ComplexFilter;
import org.gephi.filters.spi.FilterProperty;
import org.gephi.graph.api.Edge;
import org.gephi.graph.api.Graph;
import org.gephi.graph.api.Node;

/**
 * Filter that detects crossing edge and randomly deletes one of the crossing edge.
 * <p>
 * This example shows how to implement a <code>ComplexFilter<</code>. Unlike
 * <code>NodeFilter</code> or <code>EdgeFilter</code> which acts on a single element
 * this filter has the freedom to transform the whole graph structure. It is
 * suitable for recursive deletion or when the filter depends on both nodes
 * and edges. A filter works by deleting objects so the role of a complex filter
 * is to read nodes/edges and delete some of them according to the parameters.
 * <p>
 * This example doesn't have parmeters but the mechanism is the same as other
 * filters.
 * 
 * @see RemoveCrossingEdgesFilterBuilder
 * @author Mathieu Bastian
 */
public class RemoveCrossingEdgesFilter implements ComplexFilter {

    @Override
    public Graph filter(Graph graph) {

        Edge[] edges = graph.getEdges().toArray();
        for (Edge e : edges) {
            for (Edge f : edges) {
                if (e != f && graph.contains(e) && graph.contains(f) && !e.isSelfLoop() && !f.isSelfLoop()) {
                    Node s1 = e.getSource();
                    Node t1 = e.getTarget();
                    Node s2 = f.getSource();
                    Node t2 = f.getTarget();
                    double s1x = s1.getNodeData().x();
                    double s1y = s1.getNodeData().y();
                    double t1x = t1.getNodeData().x();
                    double t1y = t1.getNodeData().y();
                    double s2x = s2.getNodeData().x();
                    double s2y = s2.getNodeData().y();
                    double t2x = t2.getNodeData().x();
                    double t2y = t2.getNodeData().y();

                    double i1x = t1x - s1x;
                    double i2x = t2x - s2x;
                    double i1y = t1y - s1y;
                    double i2y = t2y - s2y;

                    double a = (-i1y * (s1x - s2x) + i1x * (s1y - s2y)) / (-i2x * i1y + i1x * i2y);
                    double b = (i2x * (s1y - s2y) - i2y * (s1x - s2x)) / (-i2x * i1y + i1x * i2y);

                    //Collision
                    if (a > 0 && a < 1 && b > 0 && b < 1) {
                        //Randomly remove an edge
                        double random = Math.random();
                        if (random > 0.5) {
                            graph.removeEdge(e);
                        } else {
                            graph.removeEdge(f);
                        }
                    }
                }
            }
        }
        return graph;
    }

    @Override
    public String getName() {
        return "Remove edge crossing";
    }

    @Override
    public FilterProperty[] getProperties() {
        return new FilterProperty[0];
    }
}
