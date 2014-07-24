/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package org.gephi.plugins.example.layout;

import java.util.ArrayList;
import java.util.List;
import org.gephi.graph.api.Graph;
import org.gephi.graph.api.GraphModel;
import org.gephi.graph.api.Node;
import org.gephi.layout.spi.Layout;
import org.gephi.layout.spi.LayoutBuilder;
import org.gephi.layout.spi.LayoutProperty;

/**
 *
 * @author Tyler
 */
public class GravityWellLayout implements Layout {
    private final LayoutBuilder builder;
    
    private GraphModel graphModel;
    
    private String indicatorLabel;
    private boolean running = false;
    private boolean interactive = true;
    
    public GravityWellLayout(GravityWellLayoutBuilder builder) {
        this.builder = builder;
    }
    
    @Override
    public void resetPropertiesValues() {
        this.indicatorLabel = "Gravity Sink";
    }
    
    @Override
    public void initAlgo() {
        running = true;
    }
    
    @Override
    public void goAlgo() {
        Graph graph = graphModel.getGraphVisible();
        graph.readLock();
        int nodeCount = graph.getNodeCount();
        Node[] nodes = graph.getNodes().toArray();
        List<Node> gravityNodes = new ArrayList<Node>();
        List<float[]> centralityValues = new ArrayList<float[]>();
        
        
        //  Gather gravity nodes first
        for (int i = 0; i < nodeCount; i++) {
            if (nodes[i].getNodeData().getLabel().startsWith(indicatorLabel)) {
                gravityNodes.add(nodes[i]);
                centralityValues.add(new float[nodeCount]);
            }
        }
        
        if (gravityNodes.size() >= 2)
        {
            //  Generate centralities
            for (int g = 0; g < gravityNodes.size(); g++) {
                for (int i = 0; i < nodeCount; i++) {
                    if (nodes[i].getNodeData().getLabel().startsWith(indicatorLabel))
                        continue;
                    
                    centralityValues.get(g)[i] = InterpretNodeCentrality(graph, nodes[i], gravityNodes.get(g));
                }
            }
            
            //  Get max centralities and max radius
            float[] maxCentralities = new float[gravityNodes.size()];
            float maxRadius = 0.0f;
            
            for (int g = 0; g < gravityNodes.size(); g++) {
                float currentMax = 0.0f;
                for (float val : centralityValues.get(g))
                    currentMax = Math.max(currentMax, val);
                maxCentralities[g] = currentMax;
                
                maxRadius = Math.max(maxRadius, gravityNodes.get(g).getNodeData().getRadius());
            }
            
            //  Apply shared centrality to each node
            for (int i = 0; i < nodeCount; i++) {
                if (nodes[i].getNodeData().getLabel().startsWith(indicatorLabel))
                    continue;
                
                float totalNormalizedCentrality = 0.0f;
                for (int g = 0; g < gravityNodes.size(); g++) {
                    if (maxCentralities[g] <= 0) continue;
                    totalNormalizedCentrality += centralityValues.get(g)[i] / maxCentralities[g] * gravityNodes.get(g).getNodeData().getRadius();
                }

                float x = 0.0f, y = 0.0f;
                for (int g = 0; g < gravityNodes.size(); g++) {
                    
                    Node gravityNode = gravityNodes.get(g);
                    
                    if (maxCentralities[g] <= 0)
                        continue;
                    
                    //  The centrality value of this node, given the gravity node's centrality function, over the
                    //      max centrality found for this node's gravity function.
                    float normalizedCurrentCentrality = (centralityValues.get(g)[i] / maxCentralities[g]) * gravityNode.getNodeData().getRadius() / totalNormalizedCentrality;
                    
                    
                    x += gravityNode.getNodeData().x() * normalizedCurrentCentrality;
                    y += gravityNode.getNodeData().y() * normalizedCurrentCentrality;
                }

                nodes[i].getNodeData().setX(x);
                nodes[i].getNodeData().setY(y);
            }
        }
        
        graph.readUnlock();
        
        if (!this.interactive)
            this.endAlgo();
    }
    
    //  Gets the centrality of targetNode, with respect to sourceGraph, using the centrality function
    //      defined by the given gravityNode. (Part of the gravityNode label)
    private float InterpretNodeCentrality(Graph sourceGraph, Node targetNode, Node gravityNode) {
        String centralityString = gravityNode.getNodeData().getLabel();
        centralityString = centralityString.substring(this.indicatorLabel.length());
        centralityString = centralityString.trim();
        
        try {
            switch (Integer.parseInt(centralityString)) {
                case (1):
                    return sourceGraph.getDegree(targetNode);

                case (2):
                    return targetNode.getNodeData().getRadius();
                    
                case (3):
                    return targetNode.getNodeData().getLabel().length();
                    
                case (4):
                    int charSum = 0;
                    String nodeLabel = targetNode.getNodeData().getLabel();
                    for (int i = 0; i < nodeLabel.length(); i++)
                        charSum += nodeLabel.charAt(i);

                    return charSum;
                    
                default:
                    return 0;
            }
        } catch (NumberFormatException e) {
            return 0;
        }
    }
    
    @Override
    public void endAlgo() {
        running = false;
    }
    
    @Override
    public boolean canAlgo() {
        return running;
    }
    
    @Override
    public void setGraphModel(GraphModel gm) {
        this.graphModel = gm;
    }
    
    @Override
    public LayoutBuilder getBuilder() {
        return builder;
    }
    
    public void setIndicatorLabel(String label) {
        indicatorLabel = label;
    }
    
    public String getIndicatorLabel() {
        return indicatorLabel;
    }
    
    public void setIsInteractive(Boolean isInteractive) {
        this.interactive = isInteractive;
    }
    
    public boolean getIsInteractive() {
        return this.interactive;
    }
    
    @Override
    public LayoutProperty[] getProperties() {
        List<LayoutProperty> properties = new ArrayList<LayoutProperty>();
        final String GRAVITYWELLLAYOUT = "Gravity Well Layout";
        
        try {
            
            properties.add(LayoutProperty.createProperty(
                this, String.class,
                "Indicator Label",
                GRAVITYWELLLAYOUT,
                "The label to search for while determining which nodes are gravity sinks.",
                "getIndicatorLabel", "setIndicatorLabel"));
            
            properties.add(LayoutProperty.createProperty(
                this, Boolean.class,
                "Interactive",
                GRAVITYWELLLAYOUT,
                "Whether or not the layout should run continuously.",
                "getIsInteractive", "setIsInteractive"));
            
        } catch (Exception e) {
            e.printStackTrace();
        }
        
        return properties.toArray(new LayoutProperty[0]);
    }
}
