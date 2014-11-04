/*_
 * Licensed Materials - Property of IBM
 * Restricted Materials of IBM
 * 5724-H82
 * (C) Copyright IBM Corp. 2007, 2008
 * US Government Users Restricted Rights - Use, duplication or disclosure
 * restricted by GSA ADP Schedule Contract with IBM Corp
 */
package com.ibm.btt.http;
import java.text.MessageFormat;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import com.ibm.btt.base.BTTLog;
import com.ibm.btt.base.BTTLogFactory;
import com.ibm.btt.base.Settings;
import com.ibm.btt.channel.ChannelConstant;
import com.ibm.btt.channel.HandlerUtils;
import com.ibm.btt.clientserver.ChannelContext;
import com.ibm.btt.clientserver.ChannelSession;
import com.ibm.btt.clientserver.DSECSSessionNotEstablishedException;
import com.ibm.btt.clientserver.DSEChannelSession;

public class JavaEstablishSessionRequest extends JavaHttpChannelRequest {
	@SuppressWarnings("unused")
	private static final java.lang.String COPYRIGHT = "Licensed Materials - Property of IBM "//$NON-NLS-1$
			+ "Restricted Materials of IBM "//$NON-NLS-1$
			+ "5724-H82 "//$NON-NLS-1$
			+ "(C) Copyright IBM Corp. 2007, 2008 All Rights Reserved. "//$NON-NLS-1$
			+ "US Government Users Restricted Rights - Use, duplication or disclosure "//$NON-NLS-1$
			+ "restricted by GSA ADP Schedule Contract with IBM Corp ";//$NON-NLS-1$

	private static final BTTLog log = BTTLogFactory.getLog(JavaEstablishSessionRequest.class.getName());

	public JavaEstablishSessionRequest(final HttpServletRequest request) {
		super(request);
	}

	/**
	 * Performs the session managment required to support HTTP and custom
	 * session management. This method updates the <I>ChannelContext</I> with
	 * the session and must provide support for using cookies and hidden form
	 * field to manage the session.
	 * <OL>
	 * <LI>Create the session if the request is for a new session. The session
	 * is new if the header or the data contains a newsession tag with the
	 * value=true</li>
	 * <LI>Create an <I>HttpSession</I> if using cookies or create a
	 * <I>DSEChannelSession</I> if not</li>
	 * <LI>Create an entry in the sessions table for a new session</li>
	 * </OL>
	 * 
	 * @param channelContext
	 *            com.ibm.btt.clientserver.ChannelContext
	 */
	@Override
	public void preProcessRequest(final ChannelContext channelContext)
			throws DSECSSessionNotEstablishedException {
		if(log.doDebug()){
			log.entry();
		}
		final boolean newSession = HandlerUtils.getCreateSessionValue(channelContext);

		if (!newSession) {
			HandlerUtils.setCreateSessionValue(channelContext);
		}

		final HttpServletRequest req = request;
		//TODO: BTT611 fortis migration session management
//		if (runInSession(channelContext)) {
		if(this.usingCookies(channelContext)){
			final HttpSession session = req.getSession(true);
			final ChannelSession cs = new HttpChannelSession(session);
			channelContext.setChannelSession(cs);
			if (log.doDebug()) {
				final String aMessage;
				final String aTID;
				aMessage = javaClientTrace.getString("JAVACLIENTINFO0010I");
				final MessageFormat msgFormat = new MessageFormat(aMessage);
				final Object[] args = new Object[1];
				args[0] = Settings.getTID();
				aTID = Settings.getTID();
				log.debug(new StringBuffer().append("TID:").append(aTID).append(" MSG:").append(msgFormat.format(args)).toString());
			}
		}//TODO: BTT611 fortis migration session management
		else{
			DSEChannelSession channelSession = new DSEChannelSession();
			channelContext.setChannelSession(channelSession);
		}
		
		if(log.doDebug()){
			log.exit();
		}
	}
}
