//------------------------------------------------------------------------------------------------//
//                                                                                                //
//                                         T e s t R e f s                                        //
//                                                                                                //
//------------------------------------------------------------------------------------------------//
// <editor-fold defaultstate="collapsed" desc="hdr">
//
//  Copyright © Audiveris 2025. All rights reserved.
//
//  This program is free software: you can redistribute it and/or modify it under the terms of the
//  GNU Affero General Public License as published by the Free Software Foundation, either version
//  3 of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
//  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//  See the GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License along with this
//  program.  If not, see <http://www.gnu.org/licenses/>.
//------------------------------------------------------------------------------------------------//
// </editor-fold>
package org.audiveris.omr.jaxb.refs;

import org.audiveris.omr.util.BaseTestCase;
import org.audiveris.omr.util.Dumping;
import org.audiveris.omr.util.Jaxb;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;
import javax.xml.stream.XMLStreamException;

/**
 * Class <code>TestRefs</code>
 *
 * @author Hervé Bitteur
 */
public class TestRefs
        extends BaseTestCase
{
    private JAXBContext jaxbContext;

    private final File dir = new File("data/temp/test-refs");

    private final String fileName = "universe.xml";

    public void testInSequence ()
            throws JAXBException,
                   FileNotFoundException,
                   XMLStreamException,
                   IOException
    {
        marshall();
        unmarshall();
    }

    @Override
    protected void setUp ()
            throws Exception
    {
        dir.mkdirs();
        jaxbContext = JAXBContext.newInstance(Universe.class);
    }

    private void marshall ()
            throws JAXBException,
                   FileNotFoundException,
                   XMLStreamException,
                   IOException
    {
        Universe universe = new Universe();
        File target = new File(dir, fileName);
        Files.deleteIfExists(target.toPath());

        Store store = universe.store;
        Basket basket = universe.basket;

        Orange orange;
        Apple apple;

        orange = new Orange("O10", "orange 10");
        store.add(orange);
        basket.add(orange);

        apple = new Apple("A1", "pomme 1");
        store.add(apple);
        basket.add(apple);

        apple = new Apple("A2", "pomme 2");
        store.add(apple);
        basket.add(apple);

        apple = new Apple("A30", "pomme 30");
        store.add(apple);
        basket.add(apple);

        orange = new Orange("O1", "orange 1");
        store.add(orange);
        basket.add(orange);

        orange = new Orange("O2", "orange 2");
        store.add(orange);
        basket.add(orange);

        orange = new Orange("O3", "orange 3");

        store.add(orange);
        basket.add(orange);

        new Dumping().dump(universe);
        new Dumping().dump(store);
        new Dumping().dump(basket);

        System.out.println("Marshalling ...");
        Jaxb.marshal(universe, target.toPath(), jaxbContext);
        System.out.println("Marshalled   to   " + target);
        System.out.println("=========================================================");
        Jaxb.marshal(universe, System.out, jaxbContext);
    }

    private void unmarshall ()
            throws JAXBException,
                   FileNotFoundException,
                   IOException
    {
        System.out.println("=========================================================");
        System.out.println("Unmarshalling ...");

        File source = new File(dir, fileName);
        InputStream is = new FileInputStream(source);
        Unmarshaller um = jaxbContext.createUnmarshaller();

        //        MyIDResolver resolver = new MyIDResolver();
        //        um.setProperty(IDResolver.class.getName(), resolver);
        //        um.setListener(resolver.createListener());
        Universe universe = (Universe) um.unmarshal(is);
        is.close();
        System.out.println("Unmarshalled from " + source);

        new Dumping().dump(universe);
        new Dumping().dump(universe.store);
        new Dumping().dump(universe.basket);

        for (Fruit fRef : universe.store.fruits) {
            System.out.println("==================================");
            System.out.print("   " + fRef);

            if (fRef instanceof Apple) {
                Apple apple = (Apple) fRef;
                System.out.println(" apple: " + apple.name);
            }

            if (fRef instanceof Orange) {
                Orange orange = (Orange) fRef;
                System.out.println(" orange: " + orange.name);
            }
        }
    }
}
