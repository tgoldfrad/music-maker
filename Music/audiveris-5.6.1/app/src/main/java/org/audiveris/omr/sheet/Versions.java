//------------------------------------------------------------------------------------------------//
//                                                                                                //
//                                         V e r s i o n s                                        //
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
package org.audiveris.omr.sheet;

import org.audiveris.omr.OMR;
import org.audiveris.omr.WellKnowns;
import org.audiveris.omr.constant.Constant;
import org.audiveris.omr.constant.ConstantSet;
import org.audiveris.omr.step.OmrStep;
import org.audiveris.omr.ui.field.LComboBox;
import org.audiveris.omr.ui.field.LLabel;
import org.audiveris.omr.ui.util.BrowserLinkListener;
import org.audiveris.omr.ui.util.Panel;
import org.audiveris.omr.ui.util.UIUtil;
import org.audiveris.omr.util.LabeledEnum;
import org.audiveris.omr.util.Version;
import org.audiveris.omr.util.Version.UpgradeVersion;

import org.jdesktop.application.Application;
import org.jdesktop.application.ResourceMap;

import org.kohsuke.github.GHAsset;
import org.kohsuke.github.GHOrganization;
import org.kohsuke.github.GHRelease;
import org.kohsuke.github.GHRepository;
import org.kohsuke.github.GitHub;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.jgoodies.forms.builder.FormBuilder;
import com.jgoodies.forms.layout.FormLayout;

import java.awt.Color;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.io.IOException;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Collections;
import java.util.GregorianCalendar;
import java.util.List;
import java.util.Locale;

import javax.swing.AbstractAction;
import javax.swing.Action;
import javax.swing.JEditorPane;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JTextPane;

/**
 * Class <code>Versions</code> gathers key versions for upgrade checks.
 * <p>
 * It can poll Audiveris project site (GitHub) to check for availability of a more recent release
 * than the current software.
 * <p>
 * The poll can be manual (via interactive GuiActions) or automatic according to a chosen polling
 * period.
 * <p>
 * NOTA: Poll is now restricted to interactive mode, to avoid too many connections on Github site.
 *
 * @author Hervé Bitteur
 */
public abstract class Versions
{
    //~ Static fields/initializers -----------------------------------------------------------------

    private static final Constants constants = new Constants();

    private static final Logger logger = LoggerFactory.getLogger(Versions.class);

    /** Version of current Audiveris software. */
    public static final Version CURRENT_SOFTWARE = new Version(WellKnowns.TOOL_REF);

    /**
     * Version focused on better precision in inter geometry.
     * <ul>
     * <li>Migration from Point to Point2D for many inter segments (horizontal or vertical).
     * <li>Barline and Bracket now include staff line height.
     * <li>Related BarConnector and BracketConnector are shortened accordingly.
     * <li>Stem now uses thickness and vertical median line in lieu of top/bottom points.
     * <li>Ledger now uses thickness and horizontal median line.
     * </ul>
     */
    public static final UpgradeVersion INTER_GEOMETRY = new UpgradeVersion("5.2.1");

    /**
     * New handling of rest chords interleaved between beamed head chords.
     */
    public static final UpgradeVersion INTERLEAVED_RESTS = new UpgradeVersion("5.2.3")
    {
        @Override
        public boolean upgradeNeeded (SheetStub stub)
        {
            if (stub.getVersion().compareTo(this) >= 0) {
                return false;
            }

            // Interleaved rests are built and used starting at RHYTHMS step
            final OmrStep latestStep = stub.getLatestStep();

            return (latestStep != null) && latestStep.compareTo(OmrStep.RHYTHMS) >= 0;
        }
    };

    /**
     * New shape names and reordering, implying new glyph classifier file.
     * <ul>
     * <li>Additional shape names with no-oval head motif (diamond, triangle, circle, ...)
     * <li>Addition of grace notes down and small flag down
     * <li>Renaming of FLAG_x_DOWN as FLAG_x
     * <li>Renaming of FLAG_x_UP as FLAG_x_DOWN
     * </ul>
     */
    public static final UpgradeVersion DRUM_NOTATION = new UpgradeVersion("5.3-beta");

    //
    // NOTA: Add here below any new version for which some upgrade is necessary.
    //
    /**
     * Sequence of upgrade versions to check.
     * NOTA: This sequence must be manually updated when a new upgrade version is added above.
     */
    public static final List<UpgradeVersion> UPGRADE_VERSIONS = Arrays.asList(
            INTER_GEOMETRY,
            INTERLEAVED_RESTS,
            DRUM_NOTATION);

    /** Resource injection. Lazily populated on GUI. */
    private static ResourceMap resources;

    /** Localized values of Frequency enum type. Lazily populated on GUI. */
    private static LabeledEnum<Frequency>[] localeFrequencies;

    /** How to format dates. */
    private static final DateFormat DATE_FORMAT = new SimpleDateFormat("dd-MMM-yyyy", Locale.US);

    //~ Constructors -------------------------------------------------------------------------------

    /** No instance needed for this functional class. */
    @SuppressWarnings("unused")
    private Versions ()
    {
    }

    //~ Static Methods -----------------------------------------------------------------------------

    //-------//
    // check //
    //-------//
    /**
     * Check a provided (".omr" file) version against current software version.
     *
     * @param version version to check
     * @return check result
     */
    public static CheckResult check (Version version)
    {
        if (version.major > CURRENT_SOFTWARE.major) {
            return CheckResult.PROGRAM_TOO_OLD;
        }

        if (version.major == CURRENT_SOFTWARE.major && version.minor > CURRENT_SOFTWARE.minor) {
            return CheckResult.PROGRAM_TOO_OLD;
        }

        // Compatible (though book file may be upgraded automatically)
        return CheckResult.COMPATIBLE;
    }

    //-----------------//
    // considerPolling //
    //-----------------//
    /**
     * Check whether poll time has come and, if so, do poll the project site.
     */
    public static void considerPolling ()
    {
        if (isTimeToPoll()) {
            poll(false /* manual */);
        } else {
            logger.debug("Versions. Not yet time to poll");
        }
    }

    //------------------//
    // getLatestRelease //
    //------------------//
    /**
     * Retrieve the latest release available on Audiveris project site.
     *
     * @return the latest release, or null if something went wrong
     */
    public static GHRelease getLatestRelease ()
    {
        try {
            GitHub github = GitHub.connectAnonymously();

            GHOrganization organization = github.getOrganization(WellKnowns.TOOL_NAME);
            logger.debug("{}", organization);

            GHRepository repository = organization.getRepository(WellKnowns.TOOL_ID);
            logger.debug("{}", repository);

            if (repository == null) {
                logger.warn("Unknown repository: {}", WellKnowns.TOOL_ID);

                return null;
            }

            GHRelease latestRelease = repository.getLatestRelease();
            logger.debug("Latest release: {}", latestRelease);

            List<GHAsset> assets = latestRelease.listAssets().toList();

            // Remember the date this poll  was made
            Calendar now = new GregorianCalendar();
            constants.lastReleaseCheckDate.setValue(now.getTime());

            return latestRelease;
        } catch (IOException ex) {
            logger.warn("Could not connect to Audiveris project.\n{}", ex.toString());

            if (ex.getCause() != null) {
                logger.warn("Cause: {}", ex.getCause().toString());
            }

            return null;
        }
    }

    //----------------------//
    // getLocaleFrequencies //
    //----------------------//
    private static LabeledEnum<Frequency>[] getLocaleFrequencies ()
    {
        if (localeFrequencies == null) {
            localeFrequencies = LabeledEnum.values(
                    Frequency.values(),
                    getResources(),
                    Frequency.class);
        }

        return localeFrequencies;
    }

    //-----------------//
    // getNextPollDate //
    //-----------------//
    /**
     * Report the next date when project site is to be polled.
     *
     * @return next poll date
     */
    private static Calendar getNextPollDate ()
    {
        Calendar next = new GregorianCalendar();
        next.setTime(constants.lastReleaseCheckDate.getValue());

        final Frequency frequency = constants.releaseCheckFrequency.getValue();

        switch (frequency) {
            case Always -> {}
            case Daily -> next.add(Calendar.DAY_OF_MONTH, 1);
            case Weekly -> next.add(Calendar.WEEK_OF_MONTH, 1);
            case Monthly -> next.add(Calendar.MONTH, 1);
            case Yearly -> next.add(Calendar.YEAR, 1);
            case Never -> next = null;
        }

        logger.info(
                "Versions. Poll frequency: {}, next poll on: {}",
                frequency,
                (next != null) ? DATE_FORMAT.format(next.getTime()) : null);

        return next;
    }

    //--------------//
    // getResources //
    //--------------//
    private static ResourceMap getResources ()
    {
        if (resources == null) {
            resources = Application.getInstance().getContext().getResourceMap(Versions.class);
        }

        return resources;
    }

    //-------------//
    // getUpgrades //
    //-------------//
    /**
     * Report the sequence of upgrade versions to apply on the provided sheet version.
     *
     * @param sheetVersion current version of sheet
     * @return sequence of upgrades to perform, perhaps empty but never null
     */
    public static List<Version> getUpgrades (Version sheetVersion)
    {
        List<Version> list = null;

        for (Version v : UPGRADE_VERSIONS) {
            if (sheetVersion.compareTo(v) < 0) {
                if (list == null) {
                    list = new ArrayList<>();
                }

                list.add(v);
            }
        }

        return (list == null) ? Collections.emptyList() : list;
    }

    //--------------//
    // isTimeToPoll //
    //--------------//
    /**
     * Report whether the time has come to poll project site for a new release.
     *
     * @return true if so
     */
    private static boolean isTimeToPoll ()
    {
        Calendar now = new GregorianCalendar();
        Calendar next = getNextPollDate();

        if (next == null) {
            return false;
        }

        return now.compareTo(next) > 0;
    }

    //------//
    // poll //
    //------//
    /**
     * Poll the GitHub site for a new Audiveris release.
     *
     * @param manual true for a user manual poll, false for an automatic poll
     */
    public static void poll (boolean manual)
    {
        final GHRelease latest = getLatestRelease();
        final Version latestVersion = new Version(latest.getTagName().trim());

        if (Versions.CURRENT_SOFTWARE.compareTo(latestVersion) < 0) {
            logger.info("A new software release is available: {}", latestVersion);

            if (OMR.gui == null) {
                logger.info("See {}", latest.getHtmlUrl());
            } else {
                // Explicitly tell the user that check result is positive
                AbstractPanel panel = new PositivePanel(latest);
                getResources().injectComponents(panel);

                JOptionPane.showMessageDialog(
                        OMR.gui.getFrame(),
                        panel,
                        panel.getTitle(),
                        JOptionPane.INFORMATION_MESSAGE);
            }
        } else {
            logger.info("Software version is up-to-date");

            if ((OMR.gui != null) && manual) {
                // Explicitly tell the user that check result is negative
                AbstractPanel panel = new NegativePanel(latest);
                getResources().injectComponents(panel);

                JOptionPane.showMessageDialog(
                        OMR.gui.getFrame(),
                        panel,
                        panel.getTitle(),
                        JOptionPane.INFORMATION_MESSAGE);
            }
        }
    }

    //~ Inner Classes ------------------------------------------------------------------------------

    //---------------//
    // AbstractPanel //
    //---------------//
    /**
     * Common part between Negative and Positive panels.
     */
    private abstract static class AbstractPanel
            extends Panel
    {
        protected final String title;

        protected LLabel status = new LLabel(JLabel.LEFT);

        protected LLabel tag = new LLabel(JLabel.LEFT);

        protected LComboBox<LabeledEnum<Frequency>> polling = new LComboBox<>(
                getLocaleFrequencies());

        /** The JGoodies/Form layout to be used by all subclasses. */
        protected final FormLayout layout = new FormLayout(getColumnsSpec(), getRowsSpec());

        /** The JGoodies/Form builder to be used by all subclasses. */
        protected final FormBuilder builder;

        /** Handling of entered / selected values. */
        private final Action paramAction;

        AbstractPanel (String title,
                       GHRelease release)
        {
            this.title = title;

            status.setName("status");
            tag.setName("tag");
            polling.setName("polling");

            paramAction = new ParamAction();
            builder = FormBuilder.create().layout(layout).panel(this);
            defineLayout();

            tag.setText(release.getTagName());

            Frequency f = constants.releaseCheckFrequency.getValue();
            polling.setSelectedItem(LabeledEnum.valueOf(f, getLocaleFrequencies()));
        }

        private void defineLayout ()
        {
            // Status
            int r = 1; // --------------------------------
            builder.addRaw(status.getLabel()).xy(1, r);
            builder.addRaw(status.getField()).xyw(3, r, 3);

            // Tag
            r += 2; // -----------------------------------
            builder.addRaw(tag.getLabel()).xy(1, r);
            builder.addRaw(tag.getField()).xyw(3, r, 3);

            // Polling
            r += 2; // -----------------------------------
            builder.addRaw(polling.getLabel()).xy(1, r);
            builder.addRaw(polling.getField()).xyw(3, r, 1);
            polling.addActionListener(paramAction);

            setInsets(10, 10, 10, 10);
            setOpaque(true);
            setBackground(Color.WHITE);
        }

        protected String getColumnsSpec ()
        {
            return "right:pref, 5dlu, pref, 5dlu";
        }

        protected String getRowsSpec ()
        {
            return "pref, 3dlu,pref, 3dlu,pref";
        }

        String getTitle ()
        {
            return title;
        }

        private class ParamAction
                extends AbstractAction
        {
            /**
             * Method run when user presses Return/Enter in one of the parameter fields
             *
             * @param e the triggering event
             */
            @Override
            public void actionPerformed (ActionEvent e)
            {
                if (polling.getField() == e.getSource()) {
                    final Frequency newFrequency = polling.getSelectedItem().value;
                    constants.releaseCheckFrequency.setValue(newFrequency);
                }
            }
        }
    }

    //~ Enumerations -------------------------------------------------------------------------------

    //-------------//
    // CheckResult //
    //-------------//
    public enum CheckResult
    {
        COMPATIBLE,
        BOOK_TOO_OLD,
        PROGRAM_TOO_OLD;
    }

    //-----------//
    // Constants //
    //-----------//
    private static class Constants
            extends ConstantSet
    {
        private final Constant.Enum<Frequency> releaseCheckFrequency = new Constant.Enum<>(
                Frequency.class,
                Frequency.Weekly,
                "Frequency of release check");

        private final Constant.Date lastReleaseCheckDate = new Constant.Date(
                "1-Jan-2000",
                "Date when last release check was made");
    }

    //-----------//
    // Frequency //
    //-----------//
    /** Frequency for polling project site. */
    private static enum Frequency
    {
        Always,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never;
    }

    //---------------//
    // NegativePanel //
    //---------------//
    private static class NegativePanel
            extends AbstractPanel
    {
        NegativePanel (GHRelease release)
        {
            super(getResources().getString("Negative.title"), release);
            setName("PollingNegativeDialog");

            tag.setVisible(false);

            // Status
            status.setText(getResources().getString("Negative.msg"));
        }

        @Override
        protected String getColumnsSpec ()
        {
            return super.getColumnsSpec() + ", 75dlu";
        }
    }

    //---------------//
    // PositivePanel //
    //---------------//
    private static class PositivePanel
            extends AbstractPanel
    {
        private final LLabel published = new LLabel(null, null, JLabel.LEFT);

        private final JLabel urlLabel = new JLabel();

        private final JEditorPane urlField = new JEditorPane();

        private final LLabel releaseTitle = new LLabel(JLabel.LEFT);

        private final JLabel contentLabel = new JLabel();

        private final JTextPane contentField = new JTextPane();

        PositivePanel (GHRelease release)
        {
            super(getResources().getString("Positive.title"), release);
            setName("PollingPositiveDialog");

            // Status
            status.setText(getResources().getString("Positive.msg"));

            defineLayout();

            // Published
            published.setName("published");

            DateFormat dateFormat = DateFormat.getDateInstance(DateFormat.LONG);
            published.setText(dateFormat.format(release.getPublished_at()));

            // Url
            final URL url = release.getHtmlUrl();

            urlLabel.setName("urlLabel");

            urlField.setName("urlField");
            urlField.addHyperlinkListener(new BrowserLinkListener());
            urlField.setContentType("text/html");
            urlField.setEditable(false);
            urlField.setBackground(Color.WHITE);
            urlField.setText(UIUtil.htmlLink(url.toString()));

            // Release Title
            releaseTitle.setName("releaseTitle");
            releaseTitle.setText(release.getName());

            // Content
            contentLabel.setName("contentLabel");

            contentField.setName("contentField");
            contentField.setBackground(Color.WHITE);
            contentField.setEditable(false);
            contentField.setMargin(new Insets(5, 5, 5, 5));
            contentField.setText(release.getBody().trim());
        }

        private void defineLayout ()
        {
            // Published
            int r = 7; // --------------------------------
            builder.addRaw(published.getLabel()).xy(1, r);
            builder.addRaw(published.getField()).xyw(3, r, 3);

            // Url
            r += 2; // -----------------------------------
            builder.addRaw(urlLabel).xy(1, r);
            builder.addRaw(urlField).xyw(3, r, 3);

            // Title
            r += 2; // -----------------------------------
            builder.addRaw(releaseTitle.getLabel()).xy(1, r);
            builder.addRaw(releaseTitle.getField()).xyw(3, r, 3);

            // Content
            r += 2; // -----------------------------------
            builder.addRaw(contentLabel).xy(1, r);
            builder.addRaw(contentField).xyw(3, r, 3);
        }

        @Override
        protected String getColumnsSpec ()
        {
            return super.getColumnsSpec() + ", 250dlu";
        }

        @Override
        protected String getRowsSpec ()
        {
            return super.getRowsSpec() + ", 3dlu,pref, 3dlu,pref, 3dlu,pref, 3dlu,pref";
        }
    }
}
